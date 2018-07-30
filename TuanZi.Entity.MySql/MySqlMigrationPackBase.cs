using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Packs;
using TuanZi.Core.Options;
using TuanZi.Exceptions;
using TuanZi.Core;
using Microsoft.AspNetCore.Builder;

namespace TuanZi.Entity.MySql
{
    public abstract class MySqlMigrationPackBase<TDbContext> : TuanPack
        where TDbContext : DbContext
    {
        public override PackLevel Level => PackLevel.Framework;

        public override void UsePack(IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;
            using (IServiceScope scope = provider.CreateScope())
            {
                TDbContext context = CreateDbContext(scope.ServiceProvider);
                if (context != null)
                {
                    TuanOptions options = scope.ServiceProvider.GetTuanOptions();
                    TuanDbContextOptions contextOptions = options.GetDbContextOptions(context.GetType());
                    if (contextOptions != null)
                    {
                        if (contextOptions.DatabaseType != DatabaseType.MySql)
                        {
                            throw new TuanException($"Context type '{ contextOptions.DatabaseType }' is not a MySql type");
                        }
                        if (contextOptions.AutoMigrationEnabled)
                        {
                            context.CheckAndMigration();
                        }
                    }
                }
            }

            IsEnabled = true;
        }

        protected abstract TDbContext CreateDbContext(IServiceProvider scopedProvider);
    }
}