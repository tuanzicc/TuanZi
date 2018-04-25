using System;
using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.Entity
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContext GetDbContext<TEntity, TKey>() where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>;

        void Commit();
    }
}