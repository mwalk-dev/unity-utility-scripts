using System;
using System.Linq;

namespace Runtime.MPPM
{
    public static class MppmUtils
    {
        /// <summary>
        /// Is this an editor instance that is running as a MPPM clone?
        /// </summary>
        /// <returns>True if both running in the Unity Editor and running as a MPPM clone, otherwise false</returns>
        public static bool IsCloneEditor()
        {
#if !UNITY_EDITOR
            return false;
#else
            return !Unity.Multiplayer.PlayMode.CurrentPlayer.IsMainEditor;
#endif
        }

        public static bool IsMainEditor()
        {
#if !UNITY_EDITOR
            return false;
#else
            return Unity.Multiplayer.PlayMode.CurrentPlayer.IsMainEditor;
#endif
        }

        public static bool HasTag(string tag)
        {
            return Unity.Multiplayer.PlayMode.CurrentPlayer
                .Tags
                .Select(existingTag => string.Equals(existingTag, tag, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }
    }
}
