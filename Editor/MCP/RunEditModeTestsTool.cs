#if HAS_MCP && HAS_UNITY_TEST_FRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;

namespace MWUtilityScripts.MCP
{
    public static class RunEditModeTestsTool
    {
        private const string ToolName = "MWUtilityScripts.RunEditModeTests";
        internal const int DefaultTimeoutSeconds = 300;
        private const int MaximumTimeoutSeconds = 3600;

        [McpTool(
            ToolName,
            description: "Runs Unity edit mode tests through the Unity Test Framework and waits for the run to finish.",
            title: "Run edit mode tests",
            EnabledByDefault = true,
            Groups = new[] { "MWUtilityScripts" }
        )]
        public static async Task<object> HandleCommand(RunEditModeTestsParams parameters)
        {
            parameters ??= new RunEditModeTestsParams();

            if (EditorApplication.isCompiling)
                return Response.Error("COMPILATION_IN_PROGRESS: Unity is compiling scripts. Retry after compilation finishes.");

            if (EditorApplication.isUpdating)
                return Response.Error("ASSET_IMPORT_IN_PROGRESS: Unity is importing or updating assets. Retry after the editor is ready.");

            var timeoutSeconds = parameters.TimeoutSeconds <= 0
                ? DefaultTimeoutSeconds
                : Math.Min(parameters.TimeoutSeconds, MaximumTimeoutSeconds);

            var filter = new Filter
            {
                testMode = TestMode.EditMode,
                testNames = Clean(parameters.TestNames),
                groupNames = Clean(parameters.GroupNames),
                categoryNames = Clean(parameters.CategoryNames),
                assemblyNames = Clean(parameters.AssemblyNames)
            };

            var executionSettings = new ExecutionSettings(filter);
            var callback = new TestRunCallback(parameters.IncludePassedTests, parameters.IncludeStackTraces);
            var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var startedAt = DateTime.UtcNow;
            string runId = null;
            var callbacksRegistered = false;

            try
            {
                testRunnerApi.RegisterCallbacks(callback);
                callbacksRegistered = true;
                runId = testRunnerApi.Execute(executionSettings);

                var completedTask = await Task.WhenAny(
                    callback.Completion.Task,
                    Task.Delay(TimeSpan.FromSeconds(timeoutSeconds))
                );

                if (completedTask != callback.Completion.Task)
                {
                    var cancelRequested = !string.IsNullOrEmpty(runId) && TestRunnerApi.CancelTestRun(runId);
                    return Response.Error(
                        "TEST_RUN_TIMEOUT",
                        new
                        {
                            runId,
                            timeoutSeconds,
                            cancelRequested,
                            filters = CreateFilterSummary(filter)
                        });
                }

                var result = await callback.Completion.Task;
                var duration = DateTime.UtcNow - startedAt;
                var data = new
                {
                    runId,
                    durationSeconds = Math.Round(duration.TotalSeconds, 3),
                    filters = CreateFilterSummary(filter),
                    summary = new
                    {
                        result.testStatus,
                        result.resultState,
                        result.passCount,
                        result.failCount,
                        result.skipCount,
                        result.inconclusiveCount
                    },
                    failedTests = result.failedTests,
                    skippedTests = result.skippedTests,
                    inconclusiveTests = result.inconclusiveTests,
                    passedTests = result.passedTests
                };

                if (result.failCount > 0 || string.Equals(result.testStatus, TestStatus.Failed.ToString(), StringComparison.Ordinal))
                    return Response.Error("TESTS_FAILED", data);

                return Response.Success("Edit mode test run completed successfully.", data);
            }
            catch (Exception e)
            {
                return Response.Error($"UNEXPECTED_ERROR: {e.Message}");
            }
            finally
            {
                if (callbacksRegistered)
                    testRunnerApi.UnregisterCallbacks(callback);

                UnityEngine.Object.DestroyImmediate(testRunnerApi);
            }
        }

        private static string[] Clean(string[] values)
        {
            if (values == null)
                return null;

            var cleaned = values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .Distinct()
                .ToArray();

            return cleaned.Length == 0 ? null : cleaned;
        }

        private static object CreateFilterSummary(Filter filter)
        {
            return new
            {
                testMode = filter.testMode.ToString(),
                testNames = filter.testNames ?? Array.Empty<string>(),
                groupNames = filter.groupNames ?? Array.Empty<string>(),
                categoryNames = filter.categoryNames ?? Array.Empty<string>(),
                assemblyNames = filter.assemblyNames ?? Array.Empty<string>()
            };
        }

        private sealed class TestRunCallback : ICallbacks
        {
            private readonly bool includePassedTests;
            private readonly bool includeStackTraces;

            public TestRunCallback(bool includePassedTests, bool includeStackTraces)
            {
                this.includePassedTests = includePassedTests;
                this.includeStackTraces = includeStackTraces;
            }

            public TaskCompletionSource<TestRunSummary> Completion { get; } = new TaskCompletionSource<TestRunSummary>();

            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                try
                {
                    Completion.TrySetResult(TestRunSummary.FromResult(result, includePassedTests, includeStackTraces));
                }
                catch (Exception e)
                {
                    Completion.TrySetException(e);
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }

        private sealed class TestRunSummary
        {
            public string testStatus;
            public string resultState;
            public int passCount;
            public int failCount;
            public int skipCount;
            public int inconclusiveCount;
            public List<TestResultInfo> failedTests = new List<TestResultInfo>();
            public List<TestResultInfo> skippedTests = new List<TestResultInfo>();
            public List<TestResultInfo> inconclusiveTests = new List<TestResultInfo>();
            public List<TestResultInfo> passedTests;

            public static TestRunSummary FromResult(ITestResultAdaptor result, bool includePassedTests, bool includeStackTraces)
            {
                var summary = new TestRunSummary
                {
                    testStatus = result.TestStatus.ToString(),
                    resultState = result.ResultState,
                    passCount = result.PassCount,
                    failCount = result.FailCount,
                    skipCount = result.SkipCount,
                    inconclusiveCount = result.InconclusiveCount,
                    passedTests = includePassedTests ? new List<TestResultInfo>() : null
                };

                Collect(result, summary, includePassedTests, includeStackTraces);
                return summary;
            }

            private static void Collect(
                ITestResultAdaptor result,
                TestRunSummary summary,
                bool includePassedTests,
                bool includeStackTraces
            )
            {
                if (result.HasChildren)
                {
                    foreach (var child in result.Children)
                    {
                        Collect(child, summary, includePassedTests, includeStackTraces);
                    }

                    if (result.TestStatus == TestStatus.Failed && !string.IsNullOrEmpty(result.Message))
                        summary.failedTests.Add(TestResultInfo.FromResult(result, includeStackTraces));

                    return;
                }

                switch (result.TestStatus)
                {
                    case TestStatus.Failed:
                        summary.failedTests.Add(TestResultInfo.FromResult(result, includeStackTraces));
                        break;
                    case TestStatus.Skipped:
                        summary.skippedTests.Add(TestResultInfo.FromResult(result, includeStackTraces));
                        break;
                    case TestStatus.Inconclusive:
                        summary.inconclusiveTests.Add(TestResultInfo.FromResult(result, includeStackTraces));
                        break;
                    case TestStatus.Passed when includePassedTests:
                        summary.passedTests.Add(TestResultInfo.FromResult(result, includeStackTraces));
                        break;
                }
            }
        }

        private sealed class TestResultInfo
        {
            public string name;
            public string fullName;
            public string resultState;
            public double durationSeconds;
            public string message;
            public string stackTrace;
            public string output;

            public static TestResultInfo FromResult(ITestResultAdaptor result, bool includeStackTrace)
            {
                return new TestResultInfo
                {
                    name = result.Name,
                    fullName = result.FullName,
                    resultState = result.ResultState,
                    durationSeconds = Math.Round(result.Duration, 3),
                    message = string.IsNullOrEmpty(result.Message) ? null : result.Message,
                    stackTrace = includeStackTrace && !string.IsNullOrEmpty(result.StackTrace) ? result.StackTrace : null,
                    output = string.IsNullOrEmpty(result.Output) ? null : result.Output
                };
            }
        }
    }

    public record RunEditModeTestsParams
    {
        [McpDescription("Exact full names of edit mode tests or fixtures to run. Empty runs all matching edit mode tests.")]
        public string[] TestNames { get; set; }

        [McpDescription("Regex group names for matching fixtures, namespaces, or tests.")]
        public string[] GroupNames { get; set; }

        [McpDescription("NUnit category names to include in the run.")]
        public string[] CategoryNames { get; set; }

        [McpDescription("Test assembly names to include, without the .dll extension.")]
        public string[] AssemblyNames { get; set; }

        [McpDescription("Maximum time to wait for the test run in seconds.", Default = RunEditModeTestsTool.DefaultTimeoutSeconds)]
        public int TimeoutSeconds { get; set; } = RunEditModeTestsTool.DefaultTimeoutSeconds;

        [McpDescription("Whether to include passed test case names in the response.", Default = false)]
        public bool IncludePassedTests { get; set; }

        [McpDescription("Whether failed, skipped, and inconclusive test entries should include stack traces.", Default = true)]
        public bool IncludeStackTraces { get; set; } = true;
    }
}
#endif
