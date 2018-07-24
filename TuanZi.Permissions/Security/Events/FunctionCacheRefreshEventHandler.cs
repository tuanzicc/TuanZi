using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Core.Functions;
using TuanZi.Dependency;
using TuanZi.EventBuses;
using Microsoft.Extensions.DependencyInjection;

namespace TuanZi.Security.Events
{
    public class FunctionCacheRefreshEventHandler : EventHandlerBase<FunctionCacheRefreshEventData>, ITransientDependency
    {
        private readonly IServiceProvider _provider;

        public FunctionCacheRefreshEventHandler(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override void Handle(FunctionCacheRefreshEventData eventData)
        {
            if (!ServiceLocator.InScoped())
            {
                return;
            }
            IFunctionHandler functionHandler = _provider.GetService<IFunctionHandler>();
            functionHandler.RefreshCache();
        }
    }
}
