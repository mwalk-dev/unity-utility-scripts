using System;
using System.Collections.Generic;

namespace Runtime.CLR
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IReadOnlyList<T> list, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}
