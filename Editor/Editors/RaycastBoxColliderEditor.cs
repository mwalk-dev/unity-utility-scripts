using MWUtilityScripts.Physics;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MWUtilityScripts.Editor.Editors
{
    // Based on https://forum.unity.com/threads/how-can-i-access-bounding-box-size-in-a-custom-editor.966287/
    [CustomEditor(typeof(RaycastBoxCollider))]
    public class RaycastBoxColliderEditor : UnityEditor.Editor
    {
        private readonly BoxBoundsHandle _boundsHandle = new();

        protected virtual void OnSceneGUI()
        {
            RaycastBoxCollider rbc = (RaycastBoxCollider)target;

            // copy the target object's data to the handle
            _boundsHandle.size = new Vector3(
                rbc.Bounds.size.x * rbc.transform.localScale.x,
                rbc.Bounds.size.y * rbc.transform.localScale.y,
                rbc.Bounds.size.z * rbc.transform.localScale.z
            );
            _boundsHandle.center = rbc.Bounds.center;
            //_boundsHandle.center = new Vector3(
            //        rbc.Bounds.center.x * rbc.transform.localScale.x,
            //        rbc.Bounds.center.y * rbc.transform.localScale.y,
            //        rbc.Bounds.center.z * rbc.transform.localScale.z
            //    );

            // draw the handle
            EditorGUI.BeginChangeCheck();
            _boundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(rbc, "Change Bounds");

                // copy the handle's updated data back to the target object
                // FIXME DOES NOT ACCOUNT FOR SCALE
                Bounds newBounds = new() { center = _boundsHandle.center, size = _boundsHandle.size };
                rbc.Bounds = newBounds;
            }
        }
    }
}
