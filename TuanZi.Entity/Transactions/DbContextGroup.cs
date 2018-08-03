using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;


namespace TuanZi.Entity.Transactions
{
    public class DbContextGroup : IDisposable
    {
        private DbTransaction _transaction;

        public DbContextGroup()
        {
            DbContexts = new List<DbContextBase>();
        }

        public bool HasCommited { get; private set; }

        public IList<DbContextBase> DbContexts { get; }

        public void BeginOrUseTransaction(DbConnection connection)
        {
            if (_transaction?.Connection == null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                _transaction = connection.BeginTransaction();
            }
            foreach (DbContextBase context in DbContexts)
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

        public async Task BeginOrUseTransactionAsync(DbConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_transaction == null)
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }
                _transaction = connection.BeginTransaction();
            }
            foreach (DbContextBase context in DbContexts)
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

        public void BeginOrUseTransaction(DbContext context)
        {
            if (!DbContexts.Contains(context))
            {
                return;
            }
            DbConnection connection = context.Database.GetDbConnection();
            BeginOrUseTransaction(connection);
        }

        public async Task BeginOrUseTransactionAsync(DbContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Database.CurrentTransaction != null)
            {
                return;
            }
            if (!DbContexts.Contains(context))
            {
                return;
            }
            DbConnection connection = context.Database.GetDbConnection();
            await BeginOrUseTransactionAsync(connection, cancellationToken);
        }

        public void Commit()
        {
            if (HasCommited || DbContexts.Count == 0 || _transaction == null)
            {
                return;
            }

            _transaction.Commit();
            foreach (var context in DbContexts)
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

        public void Rollback()
        {
            if (_transaction?.Connection != null)
            {
                _transaction.Rollback();
            }
            foreach (var context in DbContexts)
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

        #region IDisposable

        public void Dispose()
        {
            _transaction?.Dispose();
            foreach (DbContextBase context in DbContexts)
            {
                context.Dispose();
            }
        }

        #endregion
    }
}