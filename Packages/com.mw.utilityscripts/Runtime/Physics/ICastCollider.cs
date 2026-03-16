using UnityEngine;

namespace MWUtilityScripts.Physics
{
	public interface ICastCollider
    {
        public enum RunAt
        {
            Update,
            LateUpdate,
            FixedUpdate
        }

        public event OnRaycastCollision OnRaycastCollision;
        public LayerMask LayerMask { get; set; }
        public RunAt Run { get; set; }
    }
}
