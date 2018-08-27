using System;

using TuanZi.Dependency;
using TuanZi.EventBuses;
using TuanZi.Secutiry;


namespace TuanZi.Security.Events
{
    public class DataAuthCacheRefreshEventHandler : EventHandlerBase<DataAuthCacheRefreshEventData>, ITransientDependency
    {
        public override void Handle(DataAuthCacheRefreshEventData eventData)
        {
            IDataAuthCache cache = ServiceLocator.Instance.GetService<IDataAuthCache>();
            foreach (DataAuthCacheItem cacheItem in eventData.SetItems)
            {
                cache.SetCache(cacheItem);
            }
            foreach (DataAuthCacheItem cacheItem in eventData.RemoveItems)
            {
                cache.RemoveCache(cacheItem);
            }
        }
    }
}