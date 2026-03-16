using UnityEngine;

namespace MWUtilityScripts.Components
{
    /// <summary>
    /// Activates and deactivates other GameObjects and/or MonoBehaviours in sync with itself
    /// </summary>
    public class ActiveController : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _gameObjects;

        [SerializeField]
        private MonoBehaviour[] _monoBehaviours;

        private void OnValidate()
        {
            SetActive(enabled);
        }

        private void OnEnable()
        {
            SetActive(true);
        }

        private void OnDisable()
        {
            SetActive(false);
        }

        private void SetActive(bool active)
        {
            if (_gameObjects != null)
            {
                foreach (var go in _gameObjects)
                {
                    if (go == null)
                        continue;
                    go.SetActive(active);
                }
            }

            if (_monoBehaviours != null)
            {
                foreach (var mb in _monoBehaviours)
                {
                    if (mb == null)
                        continue;
                    mb.enabled = active;
                }
            }
        }
    }
}
