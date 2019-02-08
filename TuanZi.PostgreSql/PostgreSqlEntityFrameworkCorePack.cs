using System;
using System.ComponentModel;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;


namespace TuanZi.Entity.PostgreSql
{
    public class PostgreSqlEntityFrameworkCorePack : EntityPackBase
    {
        public override PackLevel Level => PackLevel.Framework;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services = base.AddServices(services);
            services.AddScoped(typeof(ISqlExecutor<,>), typeof(PostgreSqlDapperSqlExecutor<,>));

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            bool? hasPostgreSql = provider.GetTuanOptions()?.DbContexts?.Values.Any(m => m.DatabaseType == DatabaseType.PostgreSql);
            if (hasPostgreSql == null || !hasPostgreSql.Value)
            {
                return;
            }

            base.UsePack(provider);
        }
    }
}