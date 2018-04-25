using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Z.EntityFramework.Plus;


namespace TuanZi.Entity
{
    public static class RepositoryExtensions
    {
        public static int UpdateBatchAndIntercept<TEntity, TKey>(this IRepository<TEntity, TKey> repository,
            Expression<Func<TEntity, bool>> predicate,
            Action<BatchDelete> interceptAction)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return repository.TrackQuery(predicate).Delete(interceptAction);
        }

        public static async Task<int> UpdateBatchAndInterceptAsync<TEntity, TKey>(this IRepository<TEntity, TKey> repository,
            Expression<Func<TEntity, bool>> predicate,
            Action<BatchDelete> interceptAction)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return await repository.TrackQuery(predicate).DeleteAsync(interceptAction);
        }

        public static int UpdateBatchAndIntercept<TEntity, TKey>(this IRepository<TEntity, TKey> repository,
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> updateExpression,
            Action<BatchUpdate> interceptAction)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return repository.TrackQuery(predicate).Update(updateExpression, interceptAction);
        }

        public static async Task<int> UpdateBatchAndInterceptAsync<TEntity, TKey>(this IRepository<TEntity, TKey> repository,
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> updateExpression,
            Action<BatchUpdate> interceptAction)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return await repository.TrackQuery(predicate).UpdateAsync(updateExpression, interceptAction);
        }
    }
}