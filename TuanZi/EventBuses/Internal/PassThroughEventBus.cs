using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi.EventBuses.Internal
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    internal class PassThroughEventBus : EventBusBase
    {
        public PassThroughEventBus(IHybridServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider)
            : base(serviceScopeFactory, serviceProvider)
        { }
    }
}