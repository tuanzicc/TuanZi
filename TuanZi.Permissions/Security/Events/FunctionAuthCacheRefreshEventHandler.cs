using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Dependency;
using TuanZi.EventBuses;
using TuanZi.Secutiry;
using Microsoft.Extensions.DependencyInjection;

namespace TuanZi.Security.Events
{
    public class FunctionAuthCacheRefreshEventHandler : EventHandlerBase<FunctionAuthCacheRefreshEventData>, ITransientDependency
    {
        private readonly IServiceProvider _provider;

        public FunctionAuthCacheRefreshEventHandler(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override void Handle(FunctionAuthCacheRefreshEventData eventData)
        {
            IFunctionAuthCache cache = _provider.GetService<IFunctionAuthCache>();
            if (eventData.FunctionIds.Length > 0)
            {
                cache.RemoveFunctionCaches(eventData.FunctionIds);
                foreach (Guid functionId in eventData.FunctionIds)
                {
                    cache.GetFunctionRoles(functionId);
                }
            }
            if (eventData.UserNames.Length > 0)
            {
                cache.RemoveUserCaches(eventData.UserNames);
                foreach (string userName in eventData.UserNames)
                {
                    cache.GetUserFunctions(userName);
                }
            }
        }
    }
}
