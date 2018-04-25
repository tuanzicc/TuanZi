using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Modules;
using TuanZi.Core.Options;
using TuanZi.Exceptions;


namespace TuanZi.Entity.SqlServer
{
    public abstract class SqlServerMigrationModuleBase<TDbContext> : TuanModule
        where TDbContext : DbContext
    {
        public override ModuleLevel Level => ModuleLevel.Framework;

        public override void UseModule(IServiceProvider provider)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                TDbContext context = CreateDbContext(scope.ServiceProvider);
                if (context != null)
                {
                    TuanOptions options = scope.ServiceProvider.GetTuanOptions();
                    TuanDbContextOptions contextOptions = options.GetDbContextOptions(context.GetType());
                    if (contextOptions != null)
                    {
                        if (contextOptions.DatabaseType != DatabaseType.SqlServer)
                        {
                            throw new TuanException($"Context type '{ contextOptions.DatabaseType }' is not a SqlServer type");
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