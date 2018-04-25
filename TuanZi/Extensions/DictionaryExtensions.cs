using System;
using System.Collections.Generic;


namespace TuanZi.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue>dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : default(TValue);
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue>dictionary, TKey key, Func<TValue> addFunc)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return dictionary[key] = addFunc();
        }
    }
}