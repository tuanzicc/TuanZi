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

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            HasCommited = false;
            ActiveTransactionInfos = new Dictionary<string, ActiveTransactionInfo>();
        }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactionInfos { get; }
        public bool HasCommited { get; private set; }

        public IDbContext GetDbContext<TEntity, TKey>() where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>
        {
            IEntityConfigurationTypeFinder typeFinder = _serviceProvider.GetService<IEntityConfigurationTypeFinder>();
            Type entityType = typeof(TEntity);
            Type dbContextType = typeFinder.GetDbContextTypeForEntity(entityType);

            DbContext dbContext;
            TuanDbContextOptions dbContextOptions = GetDbContextResolveOptions(dbContextType);
            DbContextResolveOptions resolveOptions = new DbContextResolveOptions(dbContextOptions);
            IDbContextResolver contextResolver = _serviceProvider.GetService<IDbContextResolver>();
            ActiveTransactionInfo transInfo = ActiveTransactionInfos.GetOrDefault(resolveOptions.ConnectionString);
            if (transInfo == null)
            {
                resolveOptions.ExistingConnection = null;
                dbContext = contextResolver.Resolve(resolveOptions);
                if (!dbContext.ExistsRelationalDatabase())
                {
                    throw new TuanException($"The database of the data context '{ dbContext.GetType().FullName }' does not exist. Please use the Migration function to create a database for data migration.");
                }

                IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
                transInfo = new ActiveTransactionInfo(transaction, dbContext);
                ActiveTransactionInfos[resolveOptions.ConnectionString] = transInfo;
            }
            else
            {
                resolveOptions.ExistingConnection = transInfo.DbContextTransaction.GetDbTransaction().Connection;
                if (transInfo.StarterDbContext.GetType() == resolveOptions.DbContextType)
                {
                    return transInfo.StarterDbContext as IDbContext;
                }
                dbContext = contextResolver.Resolve(resolveOptions);
                if (dbContext.IsRelationalTransaction())
                {
                    dbContext.Database.UseTransaction(transInfo.DbContextTransaction.GetDbTransaction());
                }
                else
                {
                    dbContext.Database.BeginTransaction();
                }
                transInfo.AttendedDbContexts.Add(dbContext);
            }
            return dbContext as IDbContext;
        }

        public void Commit()
        {
            if (HasCommited)
            {
                return;
            }
            foreach (ActiveTransactionInfo transInfo in ActiveTransactionInfos.Values)
            {
                transInfo.DbContextTransaction.Commit();

                foreach (DbContext attendedDbContext in transInfo.AttendedDbContexts)
                {
                    if (attendedDbContext.IsRelationalTransaction())
                    {
                        continue;
                    }
                    attendedDbContext.Database.CommitTransaction();
                }
            }
            HasCommited = true;
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
            foreach (ActiveTransactionInfo transInfo in ActiveTransactionInfos.Values)
            {
                transInfo.DbContextTransaction.Dispose();
                foreach (DbContext attendedDbContext in transInfo.AttendedDbContexts)
                {
                    attendedDbContext.Dispose();
                }
                transInfo.StarterDbContext.Dispose();
            }
            ActiveTransactionInfos.Clear();
        }
    }
}