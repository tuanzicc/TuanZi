using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Mapping;
using TuanZi.Secutiry;
using Z.EntityFramework.Plus;


namespace TuanZi.Entity
{
    public partial class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
       where TEntity : class, IEntity<TKey>
       where TKey : IEquatable<TKey>
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger _logger;

        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _dbContext = (DbContext)unitOfWork.GetDbContext<TEntity, TKey>();
            _dbSet = _dbContext.Set<TEntity>();
            _logger = ServiceLocator.Instance.GetLogger<Repository<TEntity, TKey>>();
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual IQueryable<TEntity> Entities
        {
            get
            {
                Expression<Func<TEntity, bool>> dataFilterExp = GetDataFilter(DataAuthOperation.Read);
                return _dbSet.AsQueryable().AsNoTracking().Where(dataFilterExp);
            }
        }

        public virtual IQueryable<TEntity> TrackEntities
        {
            get
            {
                Expression<Func<TEntity, bool>> dataFilterExp = GetDataFilter(DataAuthOperation.Read);
                return _dbSet.AsQueryable().Where(dataFilterExp);
            }
        }

        #region Synchronize

        public virtual int Insert(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));
            for (int i = 0; i < entities.Length; i++)
            {
                TEntity entity = entities[i];
                entities[i] = entity.CheckICreatedTime<TEntity, TKey>();
            }
            _dbSet.AddRange(entities);
            return _dbContext.SaveChanges();
        }

        public virtual OperationResult Insert<TInputDto>(ICollection<TInputDto> dtos,
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
                    entity = entity.CheckICreatedTime<TEntity, TKey>();
                    _dbSet.Add(entity);
                }
                catch (TuanException e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = _dbContext.SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success, "{0} record(s) added".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanges);
        }

        public virtual int Delete(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            CheckDataAuth(DataAuthOperation.Delete, entities);
            _dbSet.RemoveRange(entities);
            return _dbContext.SaveChanges();
        }

        public virtual int Delete(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            TEntity entity = _dbSet.Find(key);
            return Delete(entity);
        }

        public virtual OperationResult Delete(ICollection<TKey> ids, Action<TEntity> checkAction = null, Func<TEntity, TEntity> deleteFunc = null)
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
                    CheckDataAuth(DataAuthOperation.Delete, entity);
                    _dbSet.Remove(entity);
                }
                catch (TuanException e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(entity));
            }
            int count = _dbContext.SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success, "{0} record(s) deleted".FormatWith(ids.Count))
                : new OperationResult(OperationResultType.NoChanges);
        }

        public virtual int DeleteBatch(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));

            ((DbContextBase)_dbContext).BeginOrUseTransaction();
            return _dbSet.Where(predicate).Delete();
        }

        public virtual int Update(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            CheckDataAuth(DataAuthOperation.Update, entities);
            _dbContext.Update<TEntity, TKey>(entities);
            return _dbContext.SaveChanges();
        }

        public virtual OperationResult Update<TEditDto>(ICollection<TEditDto> dtos,
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
                    CheckDataAuth(DataAuthOperation.Update, entity);
                    _dbContext.Update<TEntity, TKey>(entity);
                }
                catch (TuanException e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = _dbContext.SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success, "{0} record(s) updated".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanges);
        }

        public virtual int UpdateBatch(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(updateExpression, nameof(updateExpression));

            ((DbContextBase)_dbContext).BeginOrUseTransaction();
            return _dbSet.Where(predicate).Update(updateExpression);
        }

        public virtual bool CheckExists(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey))
        {
            Check.NotNull(predicate, nameof(predicate));

            TKey defaultId = default(TKey);
            var entity = _dbSet.Where(predicate).Select(m => new { m.Id }).FirstOrDefault();
            bool exists = !typeof(TKey).IsValueType && ReferenceEquals(id, null) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !EntityBase<TKey>.IsKeyEqual(entity.Id, id);
            return exists;
        }

        public virtual TEntity Get(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            return _dbSet.Find(key);
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null, bool filterByDataAuth = true)
        {
            return TrackQuery(predicate, filterByDataAuth).AsNoTracking();
        }

        public virtual IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includePropertySelectors)
        {
            return TrackQuery(includePropertySelectors).AsNoTracking();
        }

        public IQueryable<TEntity> TrackQuery(Expression<Func<TEntity, bool>> predicate = null, bool filterByDataAuth = true)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (filterByDataAuth)
            {
                Expression<Func<TEntity, bool>> dataAuthExp = GetDataFilter(DataAuthOperation.Read);
                query = query.Where(dataAuthExp);
            }
            if (predicate == null)
            {
                return query;
            }
            return query.Where(predicate);
        }

        public virtual IQueryable<TEntity> TrackQuery(params Expression<Func<TEntity, object>>[] includePropertySelectors)
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

        public virtual async Task<int> InsertAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            for (int i = 0; i < entities.Length; i++)
            {
                TEntity entity = entities[i];
                entities[i] = entity.CheckICreatedTime<TEntity, TKey>();
            }

            await _dbSet.AddRangeAsync(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<OperationResult> InsertAsync<TInputDto>(ICollection<TInputDto> dtos,
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
                    entity = entity.CheckICreatedTime<TEntity, TKey>();
                    await _dbSet.AddAsync(entity);
                }
                catch (TuanException e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = await _dbContext.SaveChangesAsync();
            return count > 0
                ? new OperationResult(OperationResultType.Success, "{0} record(s) added".FormatWith(dtos.Count))
                : OperationResult.NoChanges;
        }

        public virtual async Task<int> DeleteAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            CheckDataAuth(DataAuthOperation.Delete, entities);
            _dbSet.RemoveRange(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> DeleteAsync(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            TEntity entity = await _dbSet.FindAsync(key);
            return await DeleteAsync(entity);
        }

        public virtual async Task<OperationResult> DeleteAsync(ICollection<TKey> ids,
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
                    CheckDataAuth(DataAuthOperation.Delete, entity);
                    _dbSet.Remove(entity);
                }
                catch (TuanException e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(entity));
            }
            int count = await _dbContext.SaveChangesAsync();
            return count > 0
                ? new OperationResult(OperationResultType.Success, "{0} record(s) deleted".FormatWith(ids.Count))
                : new OperationResult(OperationResultType.NoChanges);
        }

        public virtual async Task<int> DeleteBatchAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Check.NotNull(predicate, nameof(predicate));

            await ((DbContextBase)_dbContext).BeginOrUseTransactionAsync();
            return await _dbSet.Where(predicate).DeleteAsync();
        }

        public virtual async Task<int> UpdateAsync(params TEntity[] entities)
        {
            Check.NotNull(entities, nameof(entities));

            CheckDataAuth(DataAuthOperation.Update, entities);
            _dbContext.Update<TEntity, TKey>(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<OperationResult> UpdateAsync<TEditDto>(ICollection<TEditDto> dtos,
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

                    CheckDataAuth(DataAuthOperation.Update, entity);
                    _dbContext.Update<TEntity, TKey>(entity);
                }
                catch (TuanException e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                names.AddIfNotNull(GetNameValue(dto));
            }
            int count = await _dbContext.SaveChangesAsync();
            return count > 0
                ? new OperationResult(OperationResultType.Success, "{0} record(s) updated".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanges);
        }

        public virtual async Task<int> UpdateBatchAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(updateExpression, nameof(updateExpression));

            await ((DbContextBase)_dbContext).BeginOrUseTransactionAsync();
            return await _dbSet.Where(predicate).UpdateAsync(updateExpression);
        }

        public virtual async Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey))
        {
            predicate.CheckNotNull(nameof(predicate));

            TKey defaultId = default(TKey);
            var entity = await _dbSet.Where(predicate).Select(m => new { m.Id }).FirstOrDefaultAsync();
            bool exists = !typeof(TKey).IsValueType && ReferenceEquals(id, null) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !EntityBase<TKey>.IsKeyEqual(entity.Id, id);
            return exists;
        }

        public virtual async Task<TEntity> GetAsync(TKey key)
        {
            CheckEntityKey(key, nameof(key));

            return await _dbSet.FindAsync(key);
        }

        #endregion

        #region privates

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

        private static void CheckDataAuth(DataAuthOperation operation, params TEntity[] entities)
        {
            if (entities.Length == 0)
            {
                return;
            }
            Expression<Func<TEntity, bool>> exp = GetDataFilter(operation);
            Func<TEntity, bool> func = exp.Compile();
            bool flag = entities.All(func);
            if (!flag)
            {
                throw new TuanException($"Entity '{typeof(TEntity)}' data '{entities.ExpandAndToString(m => m.Id.ToString())}' has insufficient permissions for '{operation.ToDescription()}' operation");
            }
        }

        private static Expression<Func<TEntity, bool>> GetDataFilter(DataAuthOperation operation)
        {
            return FilterHelper.GetDataFilterExpression<TEntity>(operation: operation);
        }

        #endregion
    }
}