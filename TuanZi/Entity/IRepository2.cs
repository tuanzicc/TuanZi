using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using TuanZi.Data;
using TuanZi.Dependency;


namespace TuanZi.Entity
{
    public partial interface IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        #region Synchronize

        int Recycle(params TEntity[] entities);
        int Recycle(TKey key);
        int Recycle(Expression<Func<TEntity, bool>> predicate);

        int Restore(params TEntity[] entities);
        int Restore(TKey key);
        int Restore(Expression<Func<TEntity, bool>> predicate);

        
        #endregion

        #region Asynchronous

 
        Task<int> RecycleAsync(params TEntity[] entities);
        Task<int> RecycleAsync(TKey key);
        Task<int> RecycleAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> RestoreAsync(params TEntity[] entities);
        Task<int> RestoreAsync(TKey key);
        Task<int> RestoreAsync(Expression<Func<TEntity, bool>> predicate);

       
        #endregion
    }
}