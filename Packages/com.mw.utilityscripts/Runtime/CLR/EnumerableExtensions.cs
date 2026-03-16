using System.Collections.Generic;
using System.Linq;

namespace MWUtilityScripts.CLR
{
    public static class EnumerableExtensions
    {
        public static int GetSequenceHashCode<TItem>(this IEnumerable<TItem> enumerable)
        {
            if (enumerable == null)
                return 0;
            const int seedValue = 0x2D2816FE;
            const int primeNumber = 397;
            return enumerable.Aggregate(
                seedValue,
                (current, item) => (current * primeNumber) + (Equals(item, default(TItem)) ? 0 : item.GetHashCode())
            );
        }
    }
}
