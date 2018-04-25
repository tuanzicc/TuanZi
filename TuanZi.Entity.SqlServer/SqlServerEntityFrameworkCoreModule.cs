using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;


namespace TuanZi.Entity.SqlServer
{
    [DependsOnModules(typeof(EntityModule))]
    public class SqlServerEntityModule : TuanModule
    {
        public override ModuleLevel Level => ModuleLevel.Framework;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsBuilderCreator, DbContextOptionsBuilderCreator>();
            return services;
        }
    }
}