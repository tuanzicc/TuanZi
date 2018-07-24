using System;

using TuanZi.Dependency;
using TuanZi.EventBuses;


namespace TuanZi.Identity.Events
{
    public class OnlineUserCacheRemoveEventHandler : EventHandlerBase<OnlineUserCacheRemoveEventData>, ITransientDependency
    {
        public override void Handle(OnlineUserCacheRemoveEventData eventData)
        {
            IOnlineUserCache onlineUserCache = ServiceLocator.Instance.GetService<IOnlineUserCache>();
            onlineUserCache.Remove(eventData.UserName);
        }
    }
}