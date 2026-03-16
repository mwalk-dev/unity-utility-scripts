#if HAS_ANIMANCER
using Animancer;
using UnityEngine;

namespace MWUtilityScripts.Animancer
{
    public static class MixerStateExtensions
    {
        public static void SetChildParameters<TParameter>(this ManualMixerState parent, TParameter parameter)
        {
            var childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
                if (parent.GetChild(i) is MixerState<TParameter> mixer)
                    mixer.Parameter = parameter;
        }

        public static void SetChildParameters<TParent, TChild>(this MixerState<TParent> parent, TChild parameter)
        {
            var childCount = parent.ChildCount;
            for (int i = 0; i < childCount; i++)
                if (parent.GetChild(i) is MixerState<TChild> mixer)
                    mixer.Parameter = parameter;
        }

        // Eventually https://github.com/KybernetikGames/animancer/issues/257 should give us a simpler way of doing this in Animancer v8.0
        /// <summary>
        /// Uses convention to extract the appropriate component of a Vector2 mixer state and pass it into each child mixer state. The expected setup is that
        /// the idle animation lives at index 0, the forward animation lives at index 1, and subsequent animations are indexed clockwise with both 4 and 8 way
        /// animations supported.
        /// Ranges are expected to be positive for all children, e.g. walking left represents a -X component in a Vector2 but a child state will receive a
        /// positive value regardless
        /// </summary>
        /// <param name="parent"></param>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void SetChildParameters(this MixerState<Vector2> parent)
        {
            var childCount = parent.ChildCount;
            var vector = parent.Parameter;
            if (childCount == 5)
            {
                // 4 way movement
                for (int i = 1; i < 5; i++)
                {
                    if (parent.GetChild(i) is MixerState<float> mixer)
                    {
                        // Indices 1 and 3 are forward/back and use the Y component
                        mixer.Parameter = Mathf.Abs(i % 2 == 0 ? vector.x : vector.y);
                    }
                    else
                    {
                        throw new System.Exception(
                            $"Child at index {i} was expected to be a MixerState<float> but is actually a {parent.GetChild(i).GetType().Name}"
                        );
                    }
                }
            }
            else if (childCount == 9)
            {
                // 8 way movement
                throw new System.NotImplementedException();
            }
            else
            {
                throw new System.Exception(
                    $"{nameof(SetChildParameters)} expects 4 or 8 way movement starting at index 1"
                );
            }
        }
    }
}
#endif