using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Core.Options;
using TuanZi.Dependency;
using TuanZi.Exceptions;


namespace TuanZi.Entity
{
    public static class UnitOfWorkManagerExtensions
    {
        public static IDbContext GetDbContext<TEntity, TKey>(this IUnitOfWorkManager unitOfWorkManager) where TEntity : IEntity<TKey>
        {
            Type entityType = typeof(TEntity);
            return unitOfWorkManager.GetDbContext(entityType);
        }

        public static IDbContext GetDbContext(this IUnitOfWorkManager unitOfWorkManager, Type entityType)
        {
            if (!entityType.IsEntityType())
            {
                throw new TuanException($"Type '{entityType}' is not an entity type");
            }
            IUnitOfWork unitOfWork = unitOfWorkManager.GetUnitOfWork(entityType);
            return unitOfWork?.GetDbContext(entityType);
        }

        public static TuanDbContextOptions GetDbContextResolveOptions<TEntity,TKey>(this IUnitOfWorkManager unitOfWorkManager) where TEntity : IEntity<TKey>
        {
            Type entityType = typeof(TEntity);
            return unitOfWorkManager.GetDbContextResolveOptions(entityType);
        }
        
        public static TuanDbContextOptions GetDbContextResolveOptions(this IUnitOfWorkManager unitOfWorkManager, Type entityType)
        {
            Type dbContextType = unitOfWorkManager.GetDbContextType(entityType);
            TuanDbContextOptions dbContextOptions = unitOfWorkManager.ServiceProvider.GetTuanOptions()?.GetDbContextOptions(dbContextType);
            if (dbContextOptions == null)
            {
                throw new TuanException($"Unable to find configuration information for data context '{dbContextType}'");
            }
            return dbContextOptions;
        }

        public static ISqlExecutor<TEntity,TKey> GetSqlExecutor<TEntity,TKey>(this IUnitOfWorkManager unitOfWorkManager) where TEntity : IEntity<TKey>
        {
            TuanDbContextOptions options = unitOfWorkManager.GetDbContextResolveOptions(typeof(TEntity));
            DatabaseType databaseType = options.DatabaseType;
            IList<ISqlExecutor<TEntity, TKey>> executors = unitOfWorkManager.ServiceProvider.GetServices<ISqlExecutor<TEntity, TKey>>().ToList();
            return executors.FirstOrDefault(m => m.DatabaseType == databaseType);
        }
    }
}