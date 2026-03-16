using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
	/// <summary>
	/// Maintains lists of GameObjects and/or MonoBehaviours that should only be enabled in client or server builds
	/// but not both.
	/// </summary>
	public class ClientServerConditionalEnable : MonoBehaviour
    {
        [SerializeField]
        private List<Object> _clientOnly;

        [SerializeField]
        private List<Object> _serverOnly;

        private void OnValidate()
        {
            _clientOnly?.ForEach(o => SetActive(o, false));
            _serverOnly?.ForEach(o => SetActive(o, false));
        }

        private void Awake()
        {
#if UNITY_SERVER
            _serverOnly?.ForEach(o => SetActive(o, true));
#else
            _clientOnly?.ForEach(o => SetActive(o, true));
#endif
        }

        private void SetActive(Object o, bool isActive)
        {
            if (o is GameObject go)
            {
                go.SetActive(isActive);
            }
            else if (o is MonoBehaviour mb)
            {
                mb.enabled = isActive;
            }
        }
    }
}
