using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TuanZi.Core.Packs;


namespace TuanZi.Entity.MySql
{
    public class MySqlEntityPack : EntityPackBase
    {
        public override PackLevel Level => PackLevel.Framework;

        public override int Order => 1;

        public override IServiceCollection AddServices(IServiceCollection services)
        {
            services = base.AddServices(services);

            services.AddScoped(typeof(ISqlExecutor<,>), typeof(MySqlDapperSqlExecutor<,>));

            return services;
        }

        public override void UsePack(IServiceProvider provider)
        {
            bool? hasMySql = provider.GetTuanOptions()?.DbContexts?.Values.Any(m => m.DatabaseType == DatabaseType.MySql);
            if (hasMySql == null || !hasMySql.Value)
            {
                return;
            }

            base.UsePack(provider);
        }
    }
}