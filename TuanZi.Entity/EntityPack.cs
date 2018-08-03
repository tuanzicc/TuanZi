using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbContextManager, DbContextManager>();

            return services;
        }

        public override void UsePack(IApplicationBuilder app)
        {
            IEntityConfigurationTypeFinder finder = app.ApplicationServices.GetService<IEntityConfigurationTypeFinder>();
            finder?.Initialize();
            IsEnabled = true;
        }
    }
}