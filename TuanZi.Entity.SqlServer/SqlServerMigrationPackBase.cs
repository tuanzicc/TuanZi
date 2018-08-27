using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Core.Packs;
using TuanZi.Core.Options;
using TuanZi.Exceptions;
using TuanZi.Core;
using Microsoft.AspNetCore.Builder;

namespace TuanZi.Entity.SqlServer
{
    public abstract class SqlServerMigrationPackBase<TDbContext> : TuanPack
        where TDbContext : DbContext
    {
        public override PackLevel Level => PackLevel.Framework;

        public override void UsePack(IServiceProvider provider)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                ILogger logger = provider.GetService<ILoggerFactory>().CreateLogger(GetType());
                TDbContext context = CreateDbContext(scope.ServiceProvider);
                
                if (context != null)
                {
                    TuanOptions options = scope.ServiceProvider.GetTuanOptions();
                    TuanDbContextOptions contextOptions = options.GetDbContextOptions(context.GetType());
                    if (contextOptions == null)
                    {
                        logger.LogWarning($"Database context configuration for context type '{context.GetType()}' does not exist");
                        return;
                    }
                    if (contextOptions.DatabaseType != DatabaseType.SqlServer)
                    {
                        logger.LogWarning($"Context type '{ contextOptions.DatabaseType }' is not a SqlServer type");
                        return;
                    }
                    if (contextOptions.AutoMigrationEnabled)
                    {
                        context.CheckAndMigration();
                    }
                }
            }

            IsEnabled = true;
        }

        protected abstract TDbContext CreateDbContext(IServiceProvider scopedProvider);
    }
}