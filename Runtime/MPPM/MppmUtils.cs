using System;
using System.Linq;

namespace MWUtilityScripts.MPPM
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
            try
            {
                // This can apparently throw, as of 6000.3.11f1. Probably due to the project not having been
                // explicitly configured?
                return !Unity.Multiplayer.PlayMode.CurrentPlayer.IsMainEditor;
            }
            catch
            {
                return false;
            }
#endif
        }

        public static bool IsMainEditor()
        {
#if !UNITY_EDITOR
            return false;
#else
            try
            {
                return Unity.Multiplayer.PlayMode.CurrentPlayer.IsMainEditor;
            }
            catch
            {
                return true;
            }
#endif
        }

        public static bool HasTag(string tag)
        {
            try
            {
                return Unity
                    .Multiplayer.PlayMode.CurrentPlayer.Tags.Select(existingTag =>
                        string.Equals(existingTag, tag, StringComparison.InvariantCultureIgnoreCase)
                    )
                    .FirstOrDefault();
            }
            catch
            {
                return false;
            }
        }
    }
}
