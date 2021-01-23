using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Common.Utilities
{
    public static class EnumerableExtensions
    {
        public static void Do<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T obj in sequence)
                action(obj);
        }        
    }
}
