using System.Linq;
using UnityEditor;

namespace Editor
{
    public static class EditorBuildUtil
    {
        public static void AddScenesToBuild(params string[] scenePaths)
        {
            EditorBuildSettings.scenes = EditorBuildSettings
                .scenes.Union(scenePaths.Select(x => new EditorBuildSettingsScene(x, true)))
                .ToArray();
        }

        public static void RemoveScenesFromBuild(params string[] scenePaths)
        {
            EditorBuildSettings.scenes = EditorBuildSettings
                .scenes.Where(scene => !scenePaths.Contains(scene.path))
                .ToArray();
        }
    }
}
