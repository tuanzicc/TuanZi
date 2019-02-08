using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Entity
{
    public interface IUnitOfWorkManager : IDisposable
    {
        IServiceProvider ServiceProvider { get; }

        bool HasCommited { get; }

        IUnitOfWork GetUnitOfWork<TEntity, TKey>() where TEntity : IEntity<TKey>;

        IUnitOfWork GetUnitOfWork(Type entityType);

        Type GetDbContextType(Type entityType);

        void Commit();
    }
}
