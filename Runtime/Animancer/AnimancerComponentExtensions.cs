#if HAS_ANIMANCER
using Animancer;
using UnityEngine;

namespace MWUtilityScripts.Animancer
{
    public static class AnimancerComponentExtensions
    {
        public const int FULL_BODY_ACTION_LAYER = 1;
        public const int UPPER_BODY_ACTION_LAYER = 2;
        public const int LOWER_BODY_ACTION_LAYER = 3;

        private static AnimancerState PlayOnLayer(this AnimancerComponent cmp, int layer, AnimancerState state)
        {
            Debug.Assert(
                cmp.Layers.Count >= layer + 1,
                $"Attempted to play on layer {layer} without the layer already existing. This means the layer has no layer mask set and it is required."
            );
            var s = cmp.Layers[layer].Play(state);
            s.NormalizedTime = 0f;
            return s;
        }

        private static AnimancerState PlayOnLayer(this AnimancerComponent cmp, int layer, ITransition t)
        {
            Debug.Assert(
                cmp.Layers.Count >= layer + 1,
                $"Attempted to play on layer {layer} without the layer already existing. This means the layer has no layer mask set and it is required."
            );
            var s = cmp.Layers[layer].Play(t);
            s.NormalizedTime = 0f;
            return s;
        }

        private static void StopOnLayer(this AnimancerComponent cmp, int layer, float fadeDuration)
        {
            cmp.Layers[layer].StartFade(0f, fadeDuration);
        }

        public static AnimancerState PlayFullBodyAction(this AnimancerComponent cmp, AnimancerState s) =>
            cmp.PlayOnLayer(FULL_BODY_ACTION_LAYER, s);

        public static AnimancerState PlayFullBodyAction(this AnimancerComponent cmp, ITransition t) =>
            cmp.PlayOnLayer(FULL_BODY_ACTION_LAYER, t);

        public static void StopFullBodyAction(this AnimancerComponent cmp, float fadeDuration) =>
            cmp.StopOnLayer(FULL_BODY_ACTION_LAYER, fadeDuration);

        public static AnimancerState PlayUpperBodyAction(this AnimancerComponent cmp, AnimancerState s) =>
            cmp.PlayOnLayer(UPPER_BODY_ACTION_LAYER, s);

        public static AnimancerState PlayUpperBodyAction(this AnimancerComponent cmp, ITransition t) =>
            cmp.PlayOnLayer(UPPER_BODY_ACTION_LAYER, t);

        public static void StopUpperBodyAction(this AnimancerComponent cmp, float fadeDuration) =>
            cmp.StopOnLayer(UPPER_BODY_ACTION_LAYER, fadeDuration);

        public static AnimancerState PlayLowerBodyAction(this AnimancerComponent cmp, AnimancerState s) =>
            cmp.PlayOnLayer(LOWER_BODY_ACTION_LAYER, s);

        public static AnimancerState PlayLowerBodyAction(this AnimancerComponent cmp, ITransition t) =>
            cmp.PlayOnLayer(LOWER_BODY_ACTION_LAYER, t);

        public static void StopLowerBodyAction(this AnimancerComponent cmp, float fadeDuration) =>
            cmp.StopOnLayer(LOWER_BODY_ACTION_LAYER, fadeDuration);
    }
}
#endif