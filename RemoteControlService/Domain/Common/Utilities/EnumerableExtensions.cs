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

        public static void Match<T>(
            this IEnumerable<T> sequence,
            Action<T> some,
            Action none)
        {
            if (sequence.Any())
            {
                sequence.Do(obj => some(obj));
            }
            else
            {
                none();
            }

        }
    }
}
