using System;
using System.ComponentModel;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;


namespace TuanZi.Entity.SqlServer
{

    public class SqlServerEntityFrameworkCorePack : EntityPackBase
    {
        public override PackLevel Level => PackLevel.Framework;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services = base.AddServices(services);

            services.AddScoped(typeof(ISqlExecutor<,>), typeof(SqlServerDapperSqlExecutor<,>));

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            bool? hasMsSql = provider.GetTuanOptions()?.DbContexts?.Values.Any(m => m.DatabaseType == DatabaseType.SqlServer);
            if (hasMsSql == null || !hasMsSql.Value)
            {
                return;
            }

            base.UsePack(provider);
        }
    }
}