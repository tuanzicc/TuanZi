using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;


namespace TuanZi.Entity.MySql
{
    [DependsOnModules(typeof(EntityModule))]
    public class MySqlEntityModule : TuanModule
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