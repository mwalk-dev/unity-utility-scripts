using System;
using UnityEngine;
using static MWUtilityScripts.Physics.ICastCollider;

namespace MWUtilityScripts.Physics
{
	public delegate void OnRaycastCollision(ref RaycastHit hit, ref Vector3 velocity);

    /// <summary>
    /// A utility class to emulate box-shaped collisions except using raycasts rather than actual colliders.
    /// This can be useful for small and/or fast-moving objects, such as melee weapons or projectiles.
    ///
    /// IMPORTANT: This script MUST run after anything that will affect its position, e.g. IK if being used
    /// for melee weapon hurtboxes!
    /// </summary>
    public class RaycastBoxCollider : MonoBehaviour, ICastCollider
    {
        public event OnRaycastCollision OnRaycastCollision;

        [field: SerializeField]
        public LayerMask LayerMask { get; set; }

        [field: SerializeField]
        public RunAt Run { get; set; } = RunAt.LateUpdate;

        [SerializeField]
        public Bounds Bounds = new(Vector3.zero, Vector3.one);

        // For reference:
        // https://forum.unity.com/threads/how-can-i-access-bounding-box-size-in-a-custom-editor.966287/
        [SerializeField]
        private Vector3Int _casts = new(1, 5, 1);

        private Raycaster[] _casters;

        private void OnValidate()
        {
            if (_casts.x < 1)
                _casts.x = 1;
            if (_casts.y < 1)
                _casts.y = 1;
            if (_casts.z < 1)
                _casts.z = 1;
        }

        private void Awake()
        {
            _casters = new Raycaster[_casts.x * _casts.y * _casts.z];
            var stepX = Bounds.size.x / (_casts.x + 1);
            var stepY = Bounds.size.y / (_casts.y + 1);
            var stepZ = Bounds.size.z / (_casts.z + 1);
            var i = 0;
            for (var x = 0; x < _casts.x; x++)
            {
                for (var y = 0; y < _casts.y; y++)
                {
                    for (var z = 0; z < _casts.z; z++)
                    {
                        var offset = new Vector3(
                            Bounds.min.x + (x + 1) * stepX,
                            Bounds.min.y + (y + 1) * stepY,
                            Bounds.min.z + (z + 1) * stepZ
                        );
                        _casters[i++] = new Raycaster(transform, offset, this, OnRaycastHit);
                    }
                }
            }
        }

        private void OnEnable()
        {
            for (var i = 0; i < _casters.Length; i++)
            {
                _casters[i].SamplePosition();
            }
        }

        private void FixedUpdate()
        {
            if (Run != RunAt.FixedUpdate)
                return;
            DoRaycasts();
        }

        private void Update()
        {
            if (Run != RunAt.Update)
                return;
            DoRaycasts();
        }

        private void LateUpdate()
        {
            if (Run != RunAt.LateUpdate)
                return;
            DoRaycasts();
        }

        private void DoRaycasts()
        {
            for (var i = 0; i < _casters.Length; i++)
            {
                _casters[i].Raycast();
            }
        }

        private void OnRaycastHit(Raycaster caster, RaycastHit hit, Vector3 velocity)
        {
            OnRaycastCollision?.Invoke(ref hit, ref velocity);
        }

        private class Raycaster
        {
            private readonly Transform _baseTransform;
            private readonly RaycastHit[] _raycastHits = new RaycastHit[1];
            private readonly RaycastBoxCollider _parent;
            private readonly Action<Raycaster, RaycastHit, Vector3> _hitAction;

            private Vector3 _localOffset;
            private Vector3 _lastWorldPos;

            public Raycaster(
                Transform baseTransform,
                Vector3 localOffset,
                RaycastBoxCollider parent,
                Action<Raycaster, RaycastHit, Vector3> hitAction
            )
            {
                _baseTransform = baseTransform;
                _localOffset = localOffset;
                _parent = parent;
                _hitAction = hitAction;
            }

            public void SamplePosition()
            {
                _lastWorldPos = _baseTransform.position + _baseTransform.rotation * _localOffset;
            }

            public void Raycast()
            {
                var currentWorldPos = _baseTransform.position + _baseTransform.rotation * _localOffset;
                var direction = currentWorldPos - _lastWorldPos;
                Debug.DrawRay(_lastWorldPos, direction, Color.red);
                var hits = UnityEngine.Physics.RaycastNonAlloc(
                    _lastWorldPos,
                    direction.normalized,
                    _raycastHits,
                    direction.magnitude,
                    _parent.LayerMask
                );
                _lastWorldPos = currentWorldPos;
                if (hits == 0)
                {
                    return;
                }
                _hitAction(this, _raycastHits[0], direction / Time.deltaTime);
            }
        }
    }
}
