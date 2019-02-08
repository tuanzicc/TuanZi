using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.EventBuses;


namespace TuanZi.Identity.Events
{
    public class OnlineUserCacheRemoveEventHandler : EventHandlerBase<OnlineUserCacheRemoveEventData>
    {
        private readonly IServiceProvider _provider;

        public OnlineUserCacheRemoveEventHandler(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override void Handle(OnlineUserCacheRemoveEventData eventData)
        {
            IOnlineUserCache onlineUserCache = _provider.GetService<IOnlineUserCache>();
            onlineUserCache.Remove(eventData.UserNames);
        }
    }
}