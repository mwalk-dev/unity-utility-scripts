using UnityEngine;

namespace MWUtilityScripts.Components
{
    /// <summary>
    /// A simple behavior to rotate a GameObject at a constant rate
    /// </summary>
	public class Rotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _degreesPerSecond;

        private void Update()
        {
            transform.Rotate(_degreesPerSecond * Time.deltaTime);
        }
    }
}
