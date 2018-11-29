
using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TuanZi.Core.Options;
using TuanZi.Core.Packs;

namespace TuanZi.Entity.Sqlite
{
   
    public abstract class SqliteMigrationPackBase<TDbContext> : TuanPack
        where TDbContext : DbContext
    {
      
        public override PackLevel Level => PackLevel.Framework;

        public override void UsePack(IServiceProvider provider)
        {
            TuanOptions options = provider.GetTuanOptions();
            using (IServiceScope scope = provider.CreateScope())
            {
                ILogger logger = provider.GetLogger(GetType());
                TDbContext context = CreateDbContext(scope.ServiceProvider);
                if (context != null)
                {
                    TuanDbContextOptions contextOptions = options.GetDbContextOptions(context.GetType());
                    if (contextOptions == null)
                    {
                        logger.LogWarning($"Database context configuration for context type '{context.GetType()}' does not exist");
                        return;
                    }
                    if (contextOptions.DatabaseType != DatabaseType.Sqlite)
                    {
                        logger.LogWarning($"Context type '{ contextOptions.DatabaseType }' is not a Sqlite type");
                        return;
                    }

                    if (contextOptions.AutoMigrationEnabled)
                    {
                        context.CheckAndMigration();
                        DbContextModelCache modelCache = scope.ServiceProvider.GetService<DbContextModelCache>();
                        if (modelCache != null)
                        {
                            modelCache.Set(context.GetType(), context.Model);
                        }

                        IsEnabled = true;
                    }
                }
            }
        }

        protected abstract TDbContext CreateDbContext(IServiceProvider scopedProvider);
    }
}