using System.Collections.Generic;


namespace TuanZi.Data
{
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<IDictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        public new static IDictionary<TKey, TValue> Instance { get { return Singleton<IDictionary<TKey, TValue>>.Instance; } }
    }
}