using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using TuanZi.Core.Packs;
using TuanZi.Entity.Transactions;

namespace TuanZi.Entity
{
    public class EntityPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Framework;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IEntityConfigurationTypeFinder, EntityConfigurationTypeFinder>();
            services.AddSingleton<IDbContextResolver, DbContextResolver>();
            services.AddSingleton<DbContextModelCache>();

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();

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