using System.Collections.Generic;

using TuanZi.EventBuses;
using TuanZi.Secutiry;


namespace TuanZi.Security.Events
{
    public class DataAuthCacheRefreshEventData : EventDataBase
    {
        public List<DataAuthCacheItem> CacheItems { get; set; }
    }
}