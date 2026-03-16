using UnityEngine;

namespace MWUtilityScripts.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static string GetHierarchyPath(this GameObject self) => self.transform.GetHierarchyPath();

        public static T GetOrAddComponent<T>(this GameObject self)
            where T : Component
        {
            return self.TryGetComponent<T>(out var component) ? component : self.AddComponent<T>();
        }

        public static bool TryGetComponentInParent<T>(this GameObject self, out T component)
        {
            component = self.GetComponentInParent<T>();
            return component != null;
        }
    }
}
