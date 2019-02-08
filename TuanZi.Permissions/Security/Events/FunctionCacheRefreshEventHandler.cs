using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.AspNetCore;
using TuanZi.Core.Functions;
using TuanZi.Dependency;
using TuanZi.EventBuses;


namespace TuanZi.Security.Events
{
    public class FunctionCacheRefreshEventHandler : EventHandlerBase<FunctionCacheRefreshEventData>
    {
        private readonly IServiceProvider _provider;

        public FunctionCacheRefreshEventHandler(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override void Handle(FunctionCacheRefreshEventData eventData)
        {
            if (!_provider.InHttpRequest())
            {
                return;
            }
            IFunctionHandler functionHandler = _provider.GetService<IFunctionHandler>();
            functionHandler.RefreshCache();
        }
    }
}