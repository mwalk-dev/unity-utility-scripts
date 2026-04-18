using UnityEditor;
using UnityEngine;

namespace MWUtilityScripts.Editor
{
    public static class EditorTypeDetector
    {
        [MenuItem("Tools/Utility Scripts/Log Active Editor Type")]
        public static void LogActiveEditorType()
        {
            var target = Selection.activeObject;
            if (target == null)
            {
                Debug.LogWarning("Select an object in the Project window or Hierarchy first.");
                return;
            }

            var editor = UnityEditor.Editor.CreateEditor(target);
            if (editor != null)
            {
                var editorType = editor.GetType();
                Debug.Log(
                    $"Active editor for [{target.name}] ({target.GetType().Name}): {editorType.FullName}"
                );

                // Cleanup to avoid memory leaks
                Object.DestroyImmediate(editor);
            }
            else
            {
                Debug.LogWarning($"No editor found for [{target.name}] ({target.GetType().Name}).");
            }
        }
    }
}
