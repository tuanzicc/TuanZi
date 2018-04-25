using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;
using TuanZi.EventBuses.Internal;


namespace TuanZi.EventBuses
{
    public class EventBusModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Core;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, PassThroughEventBus>();
            services.AddSingleton<IEventSubscriber>(provider => provider.GetService<IEventBus>());
            services.AddSingleton<IEventPublisher>(provider => provider.GetService<IEventBus>());

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IEventBusBuilder, EventBusBuilder>();

            return services;
        }

        public override void UseModule(IServiceProvider provider)
        {
            IEventBusBuilder builder = provider.GetService<IEventBusBuilder>();
            builder.Build();
            IsEnabled = true;
        }
    }
}