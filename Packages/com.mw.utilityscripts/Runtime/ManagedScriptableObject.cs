using UnityEditor;
using UnityEngine;

namespace MWUtilityScripts
{
    /// <summary>
    /// Note that this generally does NOT work for addressable ScriptableObjects, as the instance that has its
    /// OnBegin/OnEnd methods called will NOT be the instance loaded by addressables
    /// </summary>
    public abstract class ManagedScriptableObject : ScriptableObject
    {
        protected abstract void OnBegin();
        protected abstract void OnEnd();

#if UNITY_EDITOR
        protected void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayStateChange;
        }

        protected void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayStateChange;
        }

        void OnPlayStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                OnBegin();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                OnEnd();
            }
        }
#else
        protected void OnEnable()
        {
            OnBegin();
        }

        protected void OnDisable()
        {
            OnEnd();
        }
#endif
    }
}
