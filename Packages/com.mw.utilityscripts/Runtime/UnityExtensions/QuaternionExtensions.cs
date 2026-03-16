using UnityEngine;

namespace Runtime
{
    // From https://discussions.unity.com/t/finding-pitch-roll-yaw-from-quaternions/65684/4
    public static class QuaternionExtensions
    {
        public static float Pitch(this Quaternion q) =>
            Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z, 1 - 2 * q.x * q.x - 2 * q.z * q.z);

        public static float Roll(this Quaternion q) => Mathf.Rad2Deg * Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w);

        public static float Yaw(this Quaternion q) =>
            Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
    }
}
