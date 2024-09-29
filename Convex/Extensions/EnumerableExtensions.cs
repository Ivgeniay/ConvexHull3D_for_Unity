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
            return source.Aggregate((x, y) => keySelector(x).CompareTo(keySelector(y)) > 0 ? x : y);
        }
    }
}
