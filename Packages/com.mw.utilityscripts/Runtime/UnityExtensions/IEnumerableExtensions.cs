using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MWUtilityScripts.UnityExtensions
{
    public static class IEnumerableExtensions
    {
        public static T Closest<T>(this IEnumerable<T> items, Vector3 position)
            where T : MonoBehaviour
        {
            return items.OrderBy(x => Vector3.Distance(x.transform.position, position)).FirstOrDefault();
        }
    }
}
