using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TuanZi.Core;
using TuanZi.Core.Options;
using TuanZi.Entity.Transactions;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Reflection;

namespace TuanZi.Entity
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextResolveOptions _resolveOptions;
        private readonly List<DbContextBase> _dbContexts = new List<DbContextBase>();
        private DbTransaction _transaction;

        public UnitOfWork(IServiceProvider serviceProvider, DbContextResolveOptions resolveOptions)
        {
            _serviceProvider = serviceProvider;
            _resolveOptions = resolveOptions;
        }

        public bool HasCommited { get; private set; }

        public virtual IDbContext GetDbContext<TEntity, TKey>() where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>
        {
            Type entityType = typeof(TEntity);
            return GetDbContext(entityType);
        }

        public IDbContext GetDbContext(Type entityType)
        {
            Type baseType = typeof(IEntity<>);
            if (!entityType.IsBaseOn(baseType))
            {
                throw new TuanException($"Type '{entityType}' is not an entity type");
            }

            IEntityConfigurationTypeFinder typeFinder = _serviceProvider.GetService<IEntityConfigurationTypeFinder>();
            Type dbContextType = typeFinder.GetDbContextTypeForEntity(entityType);

            DbContextBase dbContext = _dbContexts.FirstOrDefault(m => m.GetType() == dbContextType);
            if (dbContext != null)
            {
                return dbContext;
            }
            IDbContextResolver contextResolver = _serviceProvider.GetService<IDbContextResolver>();
            dbContext = (DbContextBase)contextResolver.Resolve(_resolveOptions);
            if (!dbContext.ExistsRelationalDatabase())
            {
                throw new TuanException($"The database for the data context '{dbContext.GetType().FullName}' does not exist. Please use the Migration function to create a database for data migration.");
            }
            if (_resolveOptions.ExistingConnection == null)
            {
                _resolveOptions.ExistingConnection = dbContext.Database.GetDbConnection();
            }

            dbContext.UnitOfWork = this;
            _dbContexts.Add(dbContext);

            return dbContext;
        }

        public virtual void BeginOrUseTransaction()
        {
            if (_dbContexts.Count == 0)
            {
                return;
            }
            if (_transaction?.Connection == null)
            {
                DbConnection connection = _resolveOptions.ExistingConnection;
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                _transaction = connection.BeginTransaction();
            }

            foreach (DbContextBase context in _dbContexts)
            {
                if (context.Database.CurrentTransaction != null && context.Database.CurrentTransaction.GetDbTransaction() == _transaction)
                {
                    continue;
                }
                if (context.IsRelationalTransaction())
                {
                    context.Database.UseTransaction(_transaction);
                }
                else
                {
                    context.Database.BeginTransaction();
                }
            }

            HasCommited = false;
        }

        public virtual async Task BeginOrUseTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_dbContexts.Count == 0)
            {
                return;
            }
            if (_transaction?.Connection == null)
            {
                DbConnection connection = _resolveOptions.ExistingConnection;
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                _transaction = connection.BeginTransaction();
            }

            foreach (DbContextBase context in _dbContexts)
            {
                if (context.Database.CurrentTransaction != null && context.Database.CurrentTransaction.GetDbTransaction() == _transaction)
                {
                    continue;
                }
                if (context.IsRelationalTransaction())
                {
                    context.Database.UseTransaction(_transaction);
                }
                else
                {
                    await context.Database.BeginTransactionAsync(cancellationToken);
                }
            }

            HasCommited = false;
        }

        public virtual void Commit()
        {
            if (HasCommited || _dbContexts.Count == 0 || _transaction == null)
            {
                return;
            }

            _transaction.Commit();
            foreach (DbContextBase context in _dbContexts)
            {
                if (context.IsRelationalTransaction())
                {
                    context.Database.CurrentTransaction.Dispose();
                    continue;
                }

                context.Database.CommitTransaction();
            }

            HasCommited = true;
        }

        public virtual void Rollback()
        {
            if (_transaction?.Connection != null)
            {
                _transaction.Rollback();
            }
            foreach (var context in _dbContexts)
            {
                if (context.IsRelationalTransaction())
                {
                    CleanChanges(context);
                    if (context.Database.CurrentTransaction != null)
                    {
                        context.Database.CurrentTransaction.Rollback();
                        context.Database.CurrentTransaction.Dispose();
                    }
                    continue;
                }
                context.Database.RollbackTransaction();
            }
            HasCommited = true;
        }

        private static void CleanChanges(DbContext context)
        {
            var entries = context.ChangeTracker.Entries().ToArray();
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].State = EntityState.Detached;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            foreach (DbContextBase context in _dbContexts)
            {
                context.Dispose();
            }
        }
    }


}