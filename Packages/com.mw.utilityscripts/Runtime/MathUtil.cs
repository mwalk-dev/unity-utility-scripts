using UnityEngine;

namespace MWUtilityScripts
{
    public static class MathUtil
    {
        public static float Map(float oldMin, float oldMax, float newMin, float newMax, float value)
        {
            value = Mathf.Clamp(value, oldMin, oldMax);
            var oldRange = oldMax - oldMin;
            var newRange = newMax - newMin;
            return (value - oldMin) * newRange / oldRange + newMin;
        }

        public static float ClampAngle(float a, float min, float max)
        {
            while (max < min)
                max += 360.0f;

            while (a > max)
                a -= 360.0f;

            while (a < min)
                a += 360.0f;

            return a > max
                ? a - (max + min) * 0.5f < 180.0f
                    ? max
                    : min
                : a;
        }
    }
}
