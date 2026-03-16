using UnityEditor;

namespace Runtime
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class PlayModeUtil
    {
        public static bool IsExiting { get; private set; }

#if UNITY_EDITOR
        static PlayModeUtil()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                IsExiting = true;
            }
            else
            {
                IsExiting = false;
            }
        }
#endif
    }
}
