using System;
using UnityEngine;

namespace MWUtilityScripts.Components
{
    /// <summary>
    /// Ensures that the specified GameObject is always rotated to face the camera
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        private bool _useMainCamera;

        [SerializeField]
        private Transform _cameraTransform;

        [NonSerialized]
        private Quaternion _originalRotation;

        private void Start()
        {
            _originalRotation = transform.rotation;
        }

        private void Update()
        {
            if (_cameraTransform == null)
            {
                if (_useMainCamera && Camera.main != null)
                {
                    _cameraTransform = Camera.main.transform;
                }
                else
                {
                    Debug.LogWarning(
                        $"{nameof(Billboard)} was not assigned a camera transform, so it will be disabled",
                        this
                    );
                    enabled = false;
                    return;
                }
            }
            transform.rotation = _cameraTransform.rotation * _originalRotation;
        }
    }
}
