using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;


namespace TuanZi.Entity.SqlServer
{
    [DependsOnPacks(typeof(EntityPack))]
    public class SqlServerEntityPack : TuanPack
    {
        public override PackLevel Level => PackLevel.Framework;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsBuilderCreator, DbContextOptionsBuilderCreator>();
            return services;
        }
    }
}