using System.Collections.Generic;

using TuanZi.EventBuses;
using TuanZi.Secutiry;


namespace TuanZi.Security.Events
{
    public class DataAuthCacheRefreshEventData : EventDataBase
    {
        public IList<DataAuthCacheItem> SetItems { get; } = new List<DataAuthCacheItem>();

        public IList<DataAuthCacheItem> RemoveItems { get; } = new List<DataAuthCacheItem>();

        public bool HasData()
        {
            return SetItems.Count > 0 || RemoveItems.Count > 0;
        }
    }
}