using System;
using System.ComponentModel;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;

namespace TuanZi.Entity.Sqlite
{

    
    public class SqliteEntityPack : EntityPackBase
    {
        public override PackLevel Level => PackLevel.Framework;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services = base.AddServices(services);

            services.AddScoped(typeof(ISqlExecutor<,>), typeof(SqliteDapperSqlExecutor<,>));

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            bool? hasSqlite = provider.GetTuanOptions()?.DbContexts?.Values.Any(m => m.DatabaseType == DatabaseType.Sqlite);
            if (hasSqlite == null || !hasSqlite.Value)
            {
                return;
            }

            base.UsePack(provider);
        }
    }
}   