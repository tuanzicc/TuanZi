using System;
using System.Collections.Generic;
using System.Linq;


namespace TuanZi.Finders
{
    public abstract class FinderBase<TItem> : IFinder<TItem>
    {
        private readonly object _lockObj = new object();

        protected readonly List<TItem> ItemsCache = new List<TItem>();

        protected bool Found = false;

        public virtual TItem[] Find(Func<TItem, bool> predicate, bool fromCache = false)
        {
            return FindAll(fromCache).Where(predicate).ToArray();
        }

        public virtual TItem[] FindAll(bool fromCache = false)
        {
            lock (_lockObj)
            {
                if (fromCache && Found)
                {
                    return ItemsCache.ToArray();
                }
                TItem[] items = FindAllItems();
                Found = true;
                ItemsCache.Clear();
                ItemsCache.AddRange(items);
                return items;
            }
        }

        protected abstract TItem[] FindAllItems();
    }
}