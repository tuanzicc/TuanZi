using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Mapping;

using Z.EntityFramework.Plus;


namespace TuanZi.Entity
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _dbContext = (DbContext)unitOfWork.GetDbContext<TEntity, TKey>();
            _dbSet = _dbContext.Set<TEntity>();
        }

        public IUnitOfWork UnitOfWork { get; }

        #region Synchronize

        public int Insert(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));
            _dbSet.AddRange(entities);
            return _dbContext.SaveChanges();
        }

        public OperationResult Insert<TInputDto>(ICollection<TInputDto> dtos,
            Action<TInputDto> checkAction = null,
            Func<TInputDto, TEntity, TEntity> updateFunc = null) where TInputDto : IInputDto<TKey>
        {
            Check.NotNull(dtos, nameof(dtos));
            List<string> names = new List<string>();
            foreach (TInputDto dto in dtos)
            {
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(dto);
                    }
                    TEntity entity = dto.MapTo<TEntity>();
                    if (updateFunc != null)
                    {
                        entity = updateFunc(dto, entity);
                    }
                    _dbSet.Add(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = _dbContext.SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "'{0}' added".FormatWith(names.ExpandAndToString())
                        : "{0} record(s) added".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        public int Delete(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            _dbSet.RemoveRange(entities);
            return _dbContext.SaveChanges();
        }

        public int Delete(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            TEntity entity = _dbSet.Find(key);
            return Delete(entity);
        }

        public OperationResult Delete(ICollection<TKey> ids, Action<TEntity> checkAction = null, Func<TEntity, TEntity> deleteFunc = null)
        {
            Check.NotNull(ids, nameof(ids));
            List<string> names = new List<string>();
            foreach (TKey id in ids)
            {
                TEntity entity = _dbSet.Find(id);
                if (entity == null)
                {
                    continue;
                }
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(entity);
                    }
                    if (deleteFunc != null)
                    {
                        entity = deleteFunc(entity);
                    }
                    _dbSet.Remove(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(entity));
            }
            int count = _dbContext.SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "'{0}' deleted".FormatWith(names.ExpandAndToString())
                        : "{0} record(s) deleted".FormatWith(ids.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        public int DeleteBatch(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));

            return _dbSet.Where(predicate).Delete();
        }

        public int Update(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            _dbSet.UpdateRange(entities);
            return _dbContext.SaveChanges();
        }

        public OperationResult Update<TEditDto>(ICollection<TEditDto> dtos,
            Action<TEditDto, TEntity> checkAction = null,
            Func<TEditDto, TEntity, TEntity> updateFunc = null) where TEditDto : IInputDto<TKey>
        {
            Check.NotNull(dtos, nameof(dtos));
            List<string> names = new List<string>();
            foreach (TEditDto dto in dtos)
            {
                try
                {
                    TEntity entity = _dbSet.Find(dto.Id);
                    if (entity == null)
                    {
                        return new OperationResult(OperationResultType.QueryNull);
                    }
                    if (checkAction != null)
                    {
                        checkAction(dto, entity);
                    }
                    entity = dto.MapTo(entity);
                    if (updateFunc != null)
                    {
                        entity = updateFunc(dto, entity);
                    }
                    _dbSet.Update(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = _dbContext.SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "'{0}' updated".FormatWith(names.ExpandAndToString())
                        : "{0} record(s) updated".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        public int UpdateBatch(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(updateExpression, nameof(updateExpression));

            return _dbSet.Where(predicate).Update(updateExpression);
        }

        public bool CheckExists(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey))
        {
            Check.NotNull(predicate, nameof(predicate));

            TKey defaultId = default(TKey);
            var entity = _dbSet.Where(predicate).Select(m => new { m.Id }).FirstOrDefault();
            bool exists = (!typeof(TKey).IsValueType && ReferenceEquals(id, null)) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !entity.Id.Equals(id);
            return exists;
        }

        public TEntity Get(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            return _dbSet.Find(key);
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (predicate == null)
            {
                return query;
            }
            return query.Where(predicate);
        }

        public IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includePropertySelectors)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (includePropertySelectors != null && includePropertySelectors.Length > 0)
            {
                foreach (Expression<Func<TEntity, object>> selector in includePropertySelectors)
                {
                    query = query.Include(selector);
                }
            }
            return query.AsNoTracking();
        }

        public IQueryable<TEntity> TrackQuery(Expression<Func<TEntity, bool>> predicate = null)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (predicate == null)
            {
                return query;
            }
            return query.Where(predicate);
        }

        public IQueryable<TEntity> TrackQuery(params Expression<Func<TEntity, object>>[] includePropertySelectors)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (includePropertySelectors != null && includePropertySelectors.Length > 0)
            {
                foreach (Expression<Func<TEntity, object>> selector in includePropertySelectors)
                {
                    query = query.Include(selector);
                }
            }
            return query;
        }

        #endregion

        #region Asynchronous

        public async Task<int> InsertAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            await _dbSet.AddRangeAsync(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<OperationResult> InsertAsync<TInputDto>(ICollection<TInputDto> dtos,
            Func<TInputDto, Task> checkAction = null,
            Func<TInputDto, TEntity, Task<TEntity>> updateFunc = null) where TInputDto : IInputDto<TKey>
        {
            Check.NotNull(dtos, nameof(dtos));
            List<string> names = new List<string>();
            foreach (TInputDto dto in dtos)
            {
                try
                {
                    if (checkAction != null)
                    {
                        await checkAction(dto);
                    }
                    TEntity entity = dto.MapTo<TEntity>();
                    if (updateFunc != null)
                    {
                        entity = await updateFunc(dto, entity);
                    }
                    await _dbSet.AddAsync(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = await _dbContext.SaveChangesAsync();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "'{0}' added".FormatWith(names.ExpandAndToString())
                        : "{0} record(s) added".FormatWith(dtos.Count))
                : OperationResult.NoChanged;
        }

        public async Task<int> DeleteAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            _dbSet.RemoveRange(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            TEntity entity = await _dbSet.FindAsync(key);
            return await DeleteAsync(entity);
        }

        public async Task<OperationResult> DeleteAsync(ICollection<TKey> ids,
            Func<TEntity, Task> checkAction = null,
            Func<TEntity, Task<TEntity>> deleteFunc = null)
        {
            Check.NotNull(ids, nameof(ids));
            List<string> names = new List<string>();
            foreach (TKey id in ids)
            {
                TEntity entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    continue;
                }
                try
                {
                    if (checkAction != null)
                    {
                        await checkAction(entity);
                    }
                    if (deleteFunc != null)
                    {
                        entity = await deleteFunc(entity);
                    }
                    _dbSet.Remove(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(entity));
            }
            int count = await _dbContext.SaveChangesAsync();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "'{0}' deleted".FormatWith(names.ExpandAndToString())
                        : "{0} record(s) deleted".FormatWith(ids.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        public async Task<int> DeleteBatchAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));

            return await _dbSet.Where(predicate).DeleteAsync();
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<OperationResult> UpdateAsync<TEditDto>(ICollection<TEditDto> dtos,
            Func<TEditDto, TEntity, Task> checkAction = null,
            Func<TEditDto, TEntity, Task<TEntity>> updateFunc = null) where TEditDto : IInputDto<TKey>
        {
            List<string> names = new List<string>();
            foreach (TEditDto dto in dtos)
            {
                try
                {
                    TEntity entity = await _dbSet.FindAsync(dto.Id);
                    if (entity == null)
                    {
                        return new OperationResult(OperationResultType.QueryNull);
                    }
                    if (checkAction != null)
                    {
                        await checkAction(dto, entity);
                    }
                    entity = dto.MapTo(entity);
                    if (updateFunc != null)
                    {
                        entity = await updateFunc(dto, entity);
                    }
                    _dbSet.Update(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = await _dbContext.SaveChangesAsync();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "'{0}' updated".FormatWith(names.ExpandAndToString())
                        : "{0} record(s) updated".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        public async Task<int> UpdateBatchAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(updateExpression, nameof(updateExpression));

            return await _dbSet.Where(predicate).UpdateAsync(updateExpression);
        }

        public async Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey))
        {
            predicate.CheckNotNull(nameof(predicate));

            TKey defaultId = default(TKey);
            var entity = await _dbSet.Where(predicate).Select(m => new { m.Id }).FirstOrDefaultAsync();
            bool exists = !typeof(TKey).IsValueType && ReferenceEquals(id, null) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !entity.Id.Equals(id);
            return exists;
        }

        public async Task<TEntity> GetAsync(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            return await _dbSet.FindAsync(key);
        }

        #endregion

        #region Private Methods

        private static void CheckEntityKey(object key, string keyName)
        {
            key.CheckNotNull("key");
            keyName.CheckNotNull("keyName");

            Type type = key.GetType();
            if (type == typeof(int))
            {
                Check.GreaterThan((int)key, keyName, 0);
            }
            else if (type == typeof(string))
            {
                Check.NotNullOrEmpty((string)key, keyName);
            }
            else if (type == typeof(Guid))
            {
                ((Guid)key).CheckNotEmpty(keyName);
            }
        }

        private static string GetNameValue(object value)
        {
            dynamic obj = value;
            try
            {
                return obj.Name;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}