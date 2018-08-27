using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core;
using TuanZi.Core.Options;
using TuanZi.Entity.Transactions;
using TuanZi.Exceptions;
using TuanZi.Extensions;


namespace TuanZi.Entity
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbContextManager _dbContextMamager;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dbContextMamager = serviceProvider.GetService<IDbContextManager>();
        }

        public bool HasCommited => _dbContextMamager.HasCommited;

        public IDbContext GetDbContext<TEntity, TKey>() where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>
        {
            IEntityConfigurationTypeFinder typeFinder = _serviceProvider.GetService<IEntityConfigurationTypeFinder>();
            Type entityType = typeof(TEntity);
            Type dbContextType = typeFinder.GetDbContextTypeForEntity(entityType);
            TuanDbContextOptions dbContextOptions = GetDbContextResolveOptions(dbContextType);
            DbContextResolveOptions resolveOptions = new DbContextResolveOptions(dbContextOptions);

            DbContextBase dbContext = _dbContextMamager.Get(dbContextType, resolveOptions.ConnectionString);
            if (dbContext != null)
            {
                return dbContext;
            }
            IDbContextResolver contextResolver = _serviceProvider.GetService<IDbContextResolver>();
            dbContext = (DbContextBase)contextResolver.Resolve(resolveOptions);
            if (!dbContext.ExistsRelationalDatabase())
            {
                throw new TuanException($"The database of the data context '{ dbContext.GetType().FullName }' does not exist. Please use the Migration function to create a database for data migration.");
            }
            if (resolveOptions.ExistingConnection == null)
            {
                resolveOptions.ExistingConnection = dbContext.Database.GetDbConnection();
            }
            _dbContextMamager.Add(dbContextOptions.ConnectionString, dbContext);

            return dbContext;
        }

        public void Commit()
        {
            _dbContextMamager.Commit();
        }

        public void Rollback()
        {
            _dbContextMamager.Rollback();
        }

        private TuanDbContextOptions GetDbContextResolveOptions(Type dbContextType)
        {
            TuanDbContextOptions dbContextOptions = _serviceProvider.GetTuanOptions()?.GetDbContextOptions(dbContextType);
            if (dbContextOptions == null)
            {
                throw new TuanException($"Cannot find configuration information for data context '{ dbContextType }'");
            }
            return dbContextOptions;
        }

        public void Dispose()
        {
            _dbContextMamager.Dispose();
        }
    }
}