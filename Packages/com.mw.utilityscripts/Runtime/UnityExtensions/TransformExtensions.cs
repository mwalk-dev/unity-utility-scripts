using System;
using UnityEngine;

namespace Runtime
{
    public static class TransformExtensions
    {
        public static Transform FindRecursiveByName(this Transform self, string exactName) =>
            self.FindRecursive(child => child.name == exactName);

        public static Transform FindRecursiveByTag(this Transform self, string exactTag) =>
            self.FindRecursive(child => child.tag == exactTag);

        public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
        {
            foreach (Transform child in self)
            {
                if (selector(child))
                {
                    return child;
                }

                var finding = child.FindRecursive(selector);

                if (finding != null)
                {
                    return finding;
                }
            }

            return null;
        }

        public static string GetHierarchyPath(this Transform self)
        {
            var obj = self;
            var path = $"/{obj.name}";
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent;
                path = $"/{obj.name}" + path;
            }
            return path;
        }
    }
}
