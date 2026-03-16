using System;
using UnityEngine;
using static Runtime.Physics.ICastCollider;

namespace Runtime.Physics
{
    // Largely C&P from RaycastBoxCollider, there is probably a lot of opportunity to consolidate
    public class SpherecastCollider : MonoBehaviour, ICastCollider
    {
        public event OnRaycastCollision OnRaycastCollision;

        [field: SerializeField]
        public LayerMask LayerMask { get; set; }

        [field: SerializeField]
        public RunAt Run { get; set; } = RunAt.LateUpdate;

        [field: SerializeField]
        public float Radius { get; private set; } = 1f;

        private Spherecaster _caster;

        private void Awake()
        {
            _caster = new(transform, this, OnRaycastHit);
        }

        private void OnEnable()
        {
            _caster.SamplePosition();
        }

        private void FixedUpdate()
        {
            if (Run != RunAt.FixedUpdate)
                return;
            DoRaycast();
        }

        private void Update()
        {
            if (Run != RunAt.Update)
                return;
            DoRaycast();
        }

        private void LateUpdate()
        {
            if (Run != RunAt.LateUpdate)
                return;
            DoRaycast();
        }

        private void DoRaycast()
        {
            _caster.Raycast();
        }

        private void OnRaycastHit(Spherecaster caster, RaycastHit hit, Vector3 velocity)
        {
            OnRaycastCollision?.Invoke(ref hit, ref velocity);
        }

        private class Spherecaster
        {
            private readonly Transform _baseTransform;
            private readonly RaycastHit[] _raycastHits = new RaycastHit[10];
            private Vector3 _lastWorldPos;

            private readonly SpherecastCollider _parent;
            private readonly Action<Spherecaster, RaycastHit, Vector3> _hitAction;

            public Spherecaster(
                Transform baseTransform,
                SpherecastCollider parent,
                Action<Spherecaster, RaycastHit, Vector3> hitAction
            )
            {
                _baseTransform = baseTransform;
                _parent = parent;
                _hitAction = hitAction;
            }

            public void SamplePosition()
            {
                _lastWorldPos = _baseTransform.position;
            }

            public void Raycast()
            {
                var currentWorldPos = _baseTransform.position;
                var direction = currentWorldPos - _lastWorldPos;
                Debug.DrawRay(_lastWorldPos, direction, Color.red);
                var hits = UnityEngine.Physics.SphereCastNonAlloc(
                    _lastWorldPos,
                    _parent.Radius,
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

                Debug.Log($"[{nameof(Spherecaster)}] Detected {hits} collisions");

                for (var i = 0; i < hits; i++)
                {
                    _hitAction(this, _raycastHits[i], direction / Time.deltaTime);
                }
            }
        }
    }
}
