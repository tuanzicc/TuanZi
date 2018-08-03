using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;
using TuanZi.EventBuses.Internal;


namespace TuanZi.EventBuses
{
    public class EventBusPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override int Order => 2;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IEventHandlerTypeFinder, EventHandlerTypeFinder>();
            services.AddSingleton<IEventBus, PassThroughEventBus>();
            services.AddSingleton<IEventSubscriber>(provider => provider.GetService<IEventBus>());
            services.AddSingleton<IEventPublisher>(provider => provider.GetService<IEventBus>());

            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IEventBusBuilder, EventBusBuilder>();

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IEventBusBuilder builder = app.ApplicationServices.GetService<IEventBusBuilder>();
            builder.Build();
            IsEnabled = true;
        }
    }
}