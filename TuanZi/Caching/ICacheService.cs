using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using TuanZi.Core.Functions;
using TuanZi.Entity;
using TuanZi.Filter;


namespace TuanZi.Caching
{
    public interface ICacheService
    {
        PageResult<TResult> ToPageCache<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> pridicate,
            PageCondition pageCondition,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams);

        PageResult<TResult> ToPageCache<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> pridicate,
            PageCondition pageCondition,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams);

        List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams);

        TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams);

        List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams);

        TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams);

        List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams);

        TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams);

        List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams);

        TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams);

        List<TSource> ToCacheList<TSource>(IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams);

        TSource[] ToCacheArray<TSource>(IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams);

        List<TSource> ToCacheList<TSource>(IQueryable<TSource> source, IFunction function, params object[] keyParams);

        TSource[] ToCacheArray<TSource>(IQueryable<TSource> source, IFunction function, params object[] keyParams);

        #region OutputDto

        PageResult<TOutputDto> ToPageCache<TEntity, TOutputDto>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            int cacheSeconds = 60,
            params object[] keyParams)
            where TOutputDto : IOutputDto;

        PageResult<TOutputDto> ToPageCache<TEntity, TOutputDto>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            IFunction function,
            params object[] keyParams)
            where TOutputDto : IOutputDto;

        List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            int cacheSeconds = 60,
            params object[] keyParams);

        TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            int cacheSeconds = 60,
            params object[] keyParams);

        List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            IFunction function,
            params object[] keyParams);

        TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            IFunction function,
            params object[] keyParams);

        List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source,
            int cacheSeconds = 60,
            params object[] keyParams);

        TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source,
            int cacheSeconds = 60,
            params object[] keyParams);

        List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source,
            IFunction function,
            params object[] keyParams);

        TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source,
            IFunction function,
            params object[] keyParams);

        #endregion
    }
}