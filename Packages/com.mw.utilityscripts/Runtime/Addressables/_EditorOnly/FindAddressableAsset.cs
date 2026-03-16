// This is not an editor-only assembly because we need to be able to reference it from runtime assemblies, however
// everything in it is editor-only so failing to wrap calling code similarly will cause compilation failures
#if UNITY_EDITOR && HAS_ADDRESSABLES
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Editor.Addressables
{
    public static class FindAddressableAsset
    {
        private static readonly Dictionary<string, AsyncOperationHandle<Object>> _guidToHandle = new();

        public static SerializedObject FindAddressableSerializedObject(SerializedObject input)
        {
            if (!EditorApplication.isPlaying)
            {
                return input;
            }

            if (
                !input.targetObject
                || !AssetDatabase.Contains(input.targetObject)
                || GetAddressableAssetEntry(input.targetObject) == null
            )
                return input;
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(input.targetObject));
            if (_guidToHandle.TryGetValue(guid, out var handle) && handle.IsValid() && handle.Result != null)
            {
                return new SerializedObject(handle.Result);
            }
            else
            {
                handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Object>(guid);
                _guidToHandle[guid] = handle;
                var asset = handle.WaitForCompletion();
                return new SerializedObject(asset);
            }
        }

        private static AddressableAssetEntry GetAddressableAssetEntry(Object obj)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)), true);
        }
    }
}
#endif
