using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TuanZi.Core.Packs;
using TuanZi.Dependency;


namespace TuanZi.EventBuses
{
    public class EventBusPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Core;

        public override int Order => 2;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            IEventHandlerTypeFinder handlerTypeFinder =
                services.GetOrAddTypeFinder<IEventHandlerTypeFinder>(assemblyFinder => new EventHandlerTypeFinder(assemblyFinder));
            Type[] eventHandlerTypes = handlerTypeFinder.FindAll();
            foreach (Type handlerType in eventHandlerTypes)
            {
                services.TryAddTransient(handlerType);
            }

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            IEventBusBuilder builder = provider.GetService<IEventBusBuilder>();
            builder.Build();
            IsEnabled = true;
        }
    }
}