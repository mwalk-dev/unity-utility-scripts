using UnityEngine;

namespace MWUtilityScripts.Components
{
    public class TemporaryObjectContainer : MonoBehaviour
    {
        public static TemporaryObjectContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance();
                }
                return _instance;
            }
        }
        private static TemporaryObjectContainer _instance;

        private static TemporaryObjectContainer CreateInstance()
        {
            var go = new GameObject(nameof(TemporaryObjectContainer));

            // Making this persist across scenes simplifies object pooling by removing the possibility that some pooled object got destroyed on a scene
            // transition, but the pool reference persisted across scenes.
            go.AddComponent<DontDestroyOnLoad>();
            return go.AddComponent<TemporaryObjectContainer>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"Only one {nameof(TemporaryObjectContainer)} should ever exist!");
                return;
            }
        }
    }
}
