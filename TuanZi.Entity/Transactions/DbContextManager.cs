using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using TuanZi.Collections;
using TuanZi.Exceptions;
using TuanZi.Extensions;


namespace TuanZi.Entity.Transactions
{
    public class DbContextManager : IDbContextManager
    {
        private readonly ConcurrentDictionary<string, DbContextGroup> _groups
            = new ConcurrentDictionary<string, DbContextGroup>();

        public bool HasCommited
        {
            get { return _groups.Values.All(m => m.HasCommited); }
        }

        public DbContextBase Get(Type contextType, string connectionString = null)
        {
            if (connectionString == null)
            {
                return _groups.Values.SelectMany(m => m.DbContexts).FirstOrDefault(m => m.GetType() == contextType);
            }
            DbContextGroup group = _groups.GetOrDefault(connectionString);
            return @group?.DbContexts.FirstOrDefault(m => m.GetType() == contextType);
        }

        public void Add(string connectionString, DbContextBase context)
        {
            DbContextGroup group = _groups.GetOrAdd(connectionString, () => new DbContextGroup());
            group.DbContexts.AddIfNotExist(context);
            context.ContextGroup = group;
            _groups[connectionString] = group;
        }

        public void Remove(string connectionString, Type contextType)
        {
            DbContextGroup group = _groups.GetOrDefault(connectionString);
            DbContextBase context = group?.DbContexts.FirstOrDefault(m => m.GetType() == contextType);
            if (context == null)
            {
                return;
            }
            group.DbContexts.Remove(context);
            context.ContextGroup = null;
            if (group.DbContexts.Count == 0)
            {
                _groups.TryRemove(connectionString, out group);
                return;
            }
            _groups[connectionString] = group;
        }

        public void BeginOrUseTransaction(DbConnection connection)
        {
            DbContextGroup group = _groups.GetOrDefault(connection.ConnectionString);
            if (group == null)
            {
                throw new TuanException("The context group specifying the connection object cannot be found when the transaction is started");
            }
            group.BeginOrUseTransaction(connection);
        }

        public Task BeginOrUseTransactionAsync(DbConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            DbContextGroup group = _groups.GetOrDefault(connection.ConnectionString);
            if (group == null)
            {
                throw new TuanException("The context group specifying the connection object cannot be found when the transaction is started");
            }
            return group.BeginOrUseTransactionAsync(connection, cancellationToken);
        }

        public void Commit()
        {
            foreach (DbContextGroup group in _groups.Values)
            {
                group.Commit();
            }
        }

        public void Rollback()
        {
            foreach (DbContextGroup group in _groups.Values)
            {
                group.Rollback();
            }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            foreach (var group in _groups.Values)
            {
                group.Dispose();
            }
            _groups.Clear();
        }

        #endregion

    }
}