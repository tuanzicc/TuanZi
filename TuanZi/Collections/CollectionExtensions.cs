using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TuanZi.Data;
using TuanZi.Extensions;

namespace TuanZi.Collections
{
    public static class CollectionExtensions
    {
        public static void AddIfNotExist<T>(this ICollection<T> collection, T value)
        {
            Check.NotNull(collection, nameof(collection));
            if (!collection.Contains(value))
            {
                collection.Add(value);
            }
        }

        public static void AddIfNotNull<T>(this ICollection<T> collection, T value) where T : class
        {
            Check.NotNull(collection, nameof(collection));
            if (value != null)
            {
                collection.Add(value);
            }
        }

        public static T GetOrAdd<T>(this ICollection<T> collection, Func<T, bool> selector, Func<T> factory)
        {
            Check.NotNull(collection, nameof(collection));
            T item = collection.FirstOrDefault(selector);
            if (item == null)
            {
                item = factory();
                collection.Add(item);
            }

            return item;
        }

        public static void AddRange(this NameValueCollection initial, NameValueCollection other)
        {
            initial.CheckNotNull("initial");

            if (other == null)
                return;

            foreach (var item in other.AllKeys)
            {
                initial.Add(item, other[item]);
            }
        }

        public static void AddRange<T>(this ICollection<T> initial, IEnumerable<T> other)
        {
            if (other == null)
                return;

            var list = initial as List<T>;

            if (list != null)
            {
                list.AddRange(other);
                return;
            }

            other.Each(x => initial.Add(x));
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return (source == null || source.Count == 0);
        }
    }
}
