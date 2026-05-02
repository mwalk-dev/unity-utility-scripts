#if HAS_MCP
using System;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;
using UnityEditor.Compilation;

namespace MWUtilityScripts.MCP
{
    public static class RefreshUnityTool
    {
        [McpTool(
            "MWUtilityScripts.RefreshUnityTool",
            description: "Forces a domain reload and script recompilation without the Unity Editor needing to be focused.",
            title: "Force asset refresh and domain reload",
            EnabledByDefault = true,
            Groups = new[] { "MWUtilityScripts" }
        )]
        public static object HandleCommand()
        {
            try
            {
                AssetDatabase.Refresh(
                    ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate
                );
                CompilationPipeline.RequestScriptCompilation();

                return Response.Success("Asset refresh and domain reload completed.");
            }
            catch (Exception e)
            {
                return Response.Error($"Failure: {e.Message}");
            }
        }
    }
}
#endif
