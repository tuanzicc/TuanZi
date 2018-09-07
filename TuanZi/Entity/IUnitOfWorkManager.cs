using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Entity
{
    public interface IUnitOfWorkManager : IDisposable
    {
        bool HasCommited { get; }

        IUnitOfWork GetUnitOfWork<TEntity, TKey>() where TEntity : IEntity<TKey>;

        IUnitOfWork GetUnitOfWork(Type entityType);

        void Commit();
    }
}
