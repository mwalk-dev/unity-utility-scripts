using UnityEngine;

namespace Runtime
{
    public static class Vector3Extensions
    {
        public static Vector2 ToXZ(this Vector3 v) => new(v.x, v.z);
    }
}
