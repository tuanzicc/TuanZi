using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Extensions;
using TuanZi.Mapping;

using Z.EntityFramework.Plus;


namespace TuanZi.Entity
{

    public partial class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
    {

        #region Synchronize


        public virtual int Recycle(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));
            foreach (var entity in entities)
            {
                entity.CheckIRecycle<TEntity, TKey>(RecycleOperation.LogicDelete);
            }
            return Update(entities);
        }

        public virtual int Recycle(TKey key)
        {
            CheckEntityKey(key, nameof(key));
            TEntity entity = _dbSet.Find(key);
            return entity == null ? 0 : Recycle(entity);
        }

        public virtual int Recycle(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            TEntity[] entities = _dbSet.Where(predicate).ToArray();
            return Recycle(entities);
        }

        public virtual int Restore(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));
            foreach (var entity in entities)
            {
                entity.CheckIRecycle<TEntity, TKey>(RecycleOperation.Restore);
            }
            return Update(entities);
        }

        public virtual int Restore(TKey key)
        {
            CheckEntityKey(key, nameof(key));
            TEntity entity = _dbSet.Find(key);
            return entity == null ? 0 : Restore(entity);
        }

        public virtual int Restore(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            TEntity[] entities = _dbSet.Where(predicate).ToArray();
            return Restore(entities);
        }

        #endregion

        #region Asynchronous


        public virtual async Task<int> RecycleAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));
            foreach (var entity in entities)
            {
                entity.CheckIRecycle<TEntity, TKey>(RecycleOperation.LogicDelete);
            }
            _dbSet.UpdateRange(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> RecycleAsync(TKey key)
        {
            CheckEntityKey(key, nameof(key));
            TEntity entity = _dbSet.Find(key);
            return entity == null ? 0 : await RecycleAsync(entity);
        }

        public virtual async Task<int> RecycleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            TEntity[] entities = _dbSet.Where(predicate).ToArray();
            return await RecycleAsync(entities);
        }

        public async Task<int> RestoreAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));
            foreach (var entity in entities)
            {
                entity.CheckIRecycle<TEntity, TKey>(RecycleOperation.Restore);
            }
            _dbSet.UpdateRange(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> RestoreAsync(TKey key)
        {
            CheckEntityKey(key, nameof(key));
            TEntity entity = _dbSet.Find(key);
            return entity == null ? 0 : await RestoreAsync(entity);
        }

        public virtual async Task<int> RestoreAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));
            TEntity[] entities = _dbSet.Where(predicate).ToArray();
            return await RestoreAsync(entities);
        }

        #endregion
    }

}