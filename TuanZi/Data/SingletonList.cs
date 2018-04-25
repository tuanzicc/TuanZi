using System.Collections.Generic;


namespace TuanZi.Data
{
    public class SingletonList<T> : Singleton<IList<T>>
    {
        static SingletonList()
        {
            Singleton<IList<T>>.Instance = new List<T>();
        }

        public new static IList<T> Instance { get { return Singleton<IList<T>>.Instance; } }
    }
}