using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TuanZi.Core.Packs;
using TuanZi.Entity.Transactions;
using TuanZi.EventBuses;

namespace TuanZi.Entity
{
    [DependsOnPacks(typeof(EventBusPack))]
    public abstract class EntityPackBase : TuanPack
    {
        public override PackLevel Level => PackLevel.Framework;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            IEntityConfigurationTypeFinder finder = provider.GetService<IEntityConfigurationTypeFinder>();
            finder?.Initialize();
            IsEnabled = true;
        }
    }
}