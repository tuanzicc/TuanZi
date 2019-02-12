using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Extensions;
using TuanZi.Mapping;
using TuanZi.Secutiry;
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


        public async Task<TOutput> GetAsync<TOutput>(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            return await _dbSet.Where(m => m.Id.Equals(key)).ToOutput<TOutput>().FirstOrDefaultAsync();
        }
        public async Task<TOutput> GetFirstAsync<TOutput>(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToOutput<TOutput>().FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(IInputDto<TKey> dto, bool checkPostFiles = true)
        {
            var result = 0;
            var entity = await GetAsync(dto.Id);
            if (entity != null)
            {
                CheckDataAuth(DataAuthOperation.Update, entity);

                if (checkPostFiles)
                {
                    var context = _serviceProvider.GetService(typeof(HttpContext)) as HttpContext;
                    if (context != null && context.Request.Method == "POST")
                    {
                        var files = context.Request.Form.Files;
                        if (files.Count > 0)
                        {
                            var fileRepository = _serviceProvider.GetService(typeof(Repository<File,Guid>)) as HttpContext;
                            foreach (var file in files)
                            {
                                if (file.Length > 0)
                                {
                                    if (target.HasProperty(file.Name))
                                    {
                                        var value = AsyncRunner.RunSync(() => fileManager.NewAsync(file));
                                        target.SetPropertyValue(file.Name, value);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    entity = entity.MapTo(entity);
                }

                _dbContext.Update<TEntity, TKey>(entity);
                result = await _dbContext.SaveChangesAsync();
            }
            return result;

        }

        public async Task<int> InsertAsync(IInputDto<TKey> dto)
        {
            var entity = dto.MapTo<TEntity>();
            entity.CheckICreatedTime<TEntity, TKey>();
            await _dbSet.AddAsync(entity);
            return await _dbContext.SaveChangesAsync();
        }

    }

}