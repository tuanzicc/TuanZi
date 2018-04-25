using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;


namespace TuanZi.Entity
{
    public class EntityModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Framework;

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