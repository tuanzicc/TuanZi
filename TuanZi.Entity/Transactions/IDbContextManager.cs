using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.Entity.Transactions
{
    public interface IDbContextManager : IDisposable
    {
        bool HasCommited { get; }

        DbContextBase Get(Type contextType, string connectionString = null);

        void Add(string connectionString, DbContextBase context);

        void Remove(string connectionString, Type contextType);

        void BeginOrUseTransaction(DbConnection connection);

        Task BeginOrUseTransactionAsync(DbConnection connection, CancellationToken cancellationToken = default(CancellationToken));

        void Commit();

        void Rollback();
    }
}