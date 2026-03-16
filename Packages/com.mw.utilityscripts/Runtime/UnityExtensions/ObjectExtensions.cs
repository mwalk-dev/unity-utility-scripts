using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MWUtilityScripts.UnityExtensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// In the editor, returns true if this isn't an asset or if it is and we're able to modify it right now
        /// In the player, always returns false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool CanModifyAsset(this Object obj)
        {
#if UNITY_EDITOR
            // We can modify it if it's not an asset
            return !AssetDatabase.Contains(obj)
                // Or it's an asset BUT
                || (
                    // We're not running on a worker thread
                    !AssetDatabase.IsAssetImportWorkerProcess()
                    // We're not compiling
                    && !EditorApplication.isCompiling
                    // We're not updating
                    && !EditorApplication.isUpdating
                );
#else
            return false;
#endif
        }

        public static string GetAssetGuid(this Object obj)
        {
#if UNITY_EDITOR
            if (AssetDatabase.Contains(obj))
                return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            Debug.LogWarning($"{obj.name} is not an asset. Returning a GUID that will not be stable.");
            return Guid.NewGuid().ToString();

#else
            Debug.LogWarning(
                $"Unable to retrieve asset GUIDs at runtime. Returning a GUID for {obj.name} that will not be stable."
            );
            return Guid.NewGuid().ToString();
#endif
        }
    }
}
