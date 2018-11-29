
using System.ComponentModel;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;

namespace TuanZi.Entity.Sqlite
{
    [DependsOnPacks(typeof(EntityPack))]
    public class SqliteEntityPack : TuanPack
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