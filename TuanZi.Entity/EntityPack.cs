using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;


namespace TuanZi.Entity
{
    public class EntityPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Framework;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IEntityConfigurationTypeFinder, EntityConfigurationTypeFinder>();
            services.AddSingleton<IEntityConfigurationAssemblyFinder, EntityConfigurationAssemblyFinder>();
            services.AddSingleton<IDbContextResolver, DbContextResolver>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            return services;
        }
    }
}