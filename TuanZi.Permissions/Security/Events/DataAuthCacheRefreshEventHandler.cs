using System;

using TuanZi.Dependency;
using TuanZi.EventBuses;
using TuanZi.Secutiry;


namespace TuanZi.Security.Events
{
    public class DataAuthCacheRefreshEventHandler : EventHandlerBase<DataAuthCacheRefreshEventData>
    {
        private readonly IDataAuthCache _authCache;

        public DataAuthCacheRefreshEventHandler(IDataAuthCache authCache)
        {
            _authCache = authCache;
        }

        public override void Handle(DataAuthCacheRefreshEventData eventData)
        {
            foreach (DataAuthCacheItem cacheItem in eventData.SetItems)
            {
                _authCache.SetCache(cacheItem);
            }
            foreach (DataAuthCacheItem cacheItem in eventData.RemoveItems)
            {
                _authCache.RemoveCache(cacheItem);
            }
        }
    }
}