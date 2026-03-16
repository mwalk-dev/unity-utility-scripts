using UnityEditor;
using UnityEngine;

namespace Editor.Addressables
{
    // See https://discussions.unity.com/t/asset-is-not-released-from-ondestroy-when-exiting-play-mode/911095/3
    // for a description of the bug this is working around
    [InitializeOnLoad]
    public static class UnloadAssetBundles
    {
        static UnloadAssetBundles()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state is not (PlayModeStateChange.ExitingPlayMode or PlayModeStateChange.ExitingEditMode))
                return;
            AssetBundle.UnloadAllAssetBundles(true);
            Debug.Log($"[{nameof(UnloadAssetBundles)}] Unloaded all asset bundles due to play mode change");
        }
    }
}
