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
    public interface IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        IUnitOfWork UnitOfWork { get; }

        #region Synchronize

        int Insert(params TEntity[] entities);

        int Insert<TEditDto>(TEditDto dto) where TEditDto : IInputDto<TKey>;
        OperationResult InsertBatch<TInputDto>(ICollection<TInputDto> dtos,
            Action<TInputDto> checkAction = null,
            Func<TInputDto, TEntity, TEntity> updateFunc = null)
            where TInputDto : IInputDto<TKey>;

        int Recycle(params TEntity[] entities);
        int Recycle(TKey key);
        int Recycle(Expression<Func<TEntity, bool>> predicate);

        int Restore(params TEntity[] entities);
        int Restore(TKey key);
        int Restore(Expression<Func<TEntity, bool>> predicate);

        int Delete(params TEntity[] entities);
        int Delete(TKey key);
        OperationResult Delete(ICollection<TKey> ids, Action<TEntity> checkAction = null, Func<TEntity, TEntity> deleteFunc = null);
        int DeleteBatch(Expression<Func<TEntity, bool>> predicate);

        int Update(params TEntity[] entities);
        int Update<TEditDto>(TEditDto dto) where TEditDto : IInputDto<TKey>;
        OperationResult UpdateBatch<TEditDto>(ICollection<TEditDto> dtos,
            Action<TEditDto, TEntity> checkAction = null,
            Func<TEditDto, TEntity, TEntity> updateFunc = null)
            where TEditDto : IInputDto<TKey>;
        int UpdateBatch(Expression<Func<TEntity, bool>>predicate, Expression<Func<TEntity, TEntity>>updateExpression);
        
        bool CheckExists(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey));

        TEntity Get(TKey key);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null);
        IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includePropertySelectors);
        IQueryable<TEntity> TrackQuery(Expression<Func<TEntity, bool>>predicate = null);
        IQueryable<TEntity> TrackQuery(params Expression<Func<TEntity, object>>[] includePropertySelectors);

        #endregion

        #region Asynchronous

        Task<int> InsertAsync(params TEntity[] entities);
        Task<int> InsertAsync<TEditDto>(TEditDto dto) where TEditDto : IInputDto<TKey>;
        Task<OperationResult> InsertBatchAsync<TInputDto>(ICollection<TInputDto> dtos,
            Func<TInputDto, Task> checkAction = null,
            Func<TInputDto, TEntity, Task<TEntity>> updateFunc = null)
            where TInputDto : IInputDto<TKey>;

        Task<int> RecycleAsync(params TEntity[] entities);
        Task<int> RecycleAsync(TKey key);
        Task<int> RecycleAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> RestoreAsync(params TEntity[] entities);
        Task<int> RestoreAsync(TKey key);
        Task<int> RestoreAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> DeleteAsync(params TEntity[] entities);
        Task<int> DeleteAsync(TKey key);
        Task<OperationResult> DeleteAsync(ICollection<TKey> ids, Func<TEntity, Task> checkAction = null, Func<TEntity, Task<TEntity>> deleteFunc = null);
        Task<int> DeleteBatchAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> UpdateAsync(TEntity entity);
        Task<int> UpdateAsync<TEditDto>(TEditDto dto) where TEditDto : IInputDto<TKey>;
        Task<OperationResult> UpdateBatchAsync<TEditDto>(ICollection<TEditDto> dtos,
            Func<TEditDto, TEntity, Task> checkAction = null,
            Func<TEditDto, TEntity, Task<TEntity>> updateFunc = null)
            where TEditDto : IInputDto<TKey>;
        Task<int> UpdateBatchAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity,TEntity>>updateExpression);

        Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey));

        Task<TEntity> GetAsync(TKey key);

        #endregion
    }
}