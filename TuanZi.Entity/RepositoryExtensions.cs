using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TuanZi.Exceptions;
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

        public static IQueryable<TEntity> FromSql<TEntity, TKey>(this IRepository<TEntity, TKey> repository, string sql, params object[] parameters)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            IUnitOfWork uow = repository.UnitOfWork;
            IDbContext dbContext = uow.GetDbContext<TEntity, TKey>();
            if (!(dbContext is DbContext context))
            {
                throw new TuanException($"The parameter dbContext is type of '{dbContext.GetType()}' and cannot be converted to DbContext");
            }
            return context.Set<TEntity>().FromSql(new RawSqlString(sql), parameters);
        }
    }
}