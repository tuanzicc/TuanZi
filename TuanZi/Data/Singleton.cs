using System;
using System.Collections.Generic;


namespace TuanZi.Data
{
    public class Singleton<T> : Singleton
    {
        private static T _instance;

        public static T Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                AllSingletons[typeof(T)] = value;
            }
        }
    }


    public class Singleton
    {
        static Singleton()
        {
            if (AllSingletons == null)
            {
                AllSingletons = new Dictionary<Type, object>();
            }
        }

        public static IDictionary<Type, object> AllSingletons { get;  }
    }
}