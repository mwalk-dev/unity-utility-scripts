using UnityEngine;

namespace MWUtilityScripts.Components
{
    public class FramerateLimiter : MonoBehaviour
    {
        [Range(-1, 60), SerializeField]
        private int _targetFramerate = -1;

#if UNITY_EDITOR
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetFramerate;
        }
#endif
    }
}
