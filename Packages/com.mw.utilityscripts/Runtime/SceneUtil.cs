using UnityEngine.SceneManagement;

namespace Runtime
{
    public static class SceneUtil
    {
        /// <summary>
        /// Retrieves the scene if it is already loaded, otherwise creates a new scene with the specified name. This
        /// should only be used for temporary or otherwise special scenes that do not need to be in a consistent state
        /// between machines.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static Scene FindOrCreateScene(string sceneName)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return default;
#endif
            var result = SceneManager.GetSceneByName(sceneName);
            if (!result.IsValid())
                result = SceneManager.CreateScene(sceneName);

            return result;
        }
    }
}
