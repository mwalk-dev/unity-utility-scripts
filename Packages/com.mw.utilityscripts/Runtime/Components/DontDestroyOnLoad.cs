using UnityEngine;

namespace Runtime
{
    /// <summary>
    /// Marks this GameObject to not be destroyed on scene change
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
