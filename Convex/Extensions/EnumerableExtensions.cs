using System.Collections.Generic;
using System.Linq; 
using System;

namespace MvConvex
{
    public static class EnumerableExtensions
    {
        public static T ArgMax<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                T maxElement = enumerator.Current;
                TKey maxKey = keySelector(maxElement);

                while (enumerator.MoveNext())
                {
                    T currentElement = enumerator.Current;
                    TKey currentKey = keySelector(currentElement);

                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxElement = currentElement;
                        maxKey = currentKey;
                    }
                }

                return maxElement;
            }
        }
    }
}
