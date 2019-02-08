using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Collections;
using TuanZi.Core.Functions;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Reflection;


namespace TuanZi.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        #region Implementation of ICacheService

        public virtual PageResult<TResult> ToPageCache<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> pridicate,
            PageCondition pageCondition,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            string key = GetKey(source, pridicate, pageCondition, selector, keyParams);
            return _cache.Get(key, () => source.ToPage(pridicate, pageCondition, selector), cacheSeconds);
        }

        public virtual PageResult<TResult> ToPageCache<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> pridicate,
            PageCondition pageCondition,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            string key = GetKey(source, pridicate, pageCondition, selector, keyParams);
            return _cache.Get(key, () => source.ToPage(pridicate, pageCondition, selector), function);
        }

        public virtual List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return ToCacheList(source.Where(predicate), selector, cacheSeconds, keyParams);
        }

        public virtual TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return ToCacheArray(source.Where(predicate), selector, cacheSeconds, keyParams);
        }

        public virtual List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            return ToCacheList(source.Where(predicate), selector, function, keyParams);
        }

        public virtual TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            return ToCacheArray(source.Where(predicate), selector, function, keyParams);
        }

        public virtual List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            string key = GetKey(source, selector, keyParams);
            return _cache.Get(key, () => source.Select(selector).ToList(), cacheSeconds);
        }

        public virtual TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            string key = GetKey(source, selector, keyParams);
            return _cache.Get(key, () => source.Select(selector).ToArray(), cacheSeconds);
        }

        public virtual List<TResult> ToCacheList<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            string key = GetKey(source, selector, keyParams);
            return _cache.Get(key, () => source.Select(selector).ToList(), function);
        }

        public virtual TResult[] ToCacheArray<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            string key = GetKey(source, selector, keyParams);
            return _cache.Get(key, () => source.Select(selector).ToArray(), function);
        }

        public virtual List<TSource> ToCacheList<TSource>(IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            string key = GetKey(source.Expression, keyParams);
            return _cache.Get(key, source.ToList, cacheSeconds);
        }

        public virtual TSource[] ToCacheArray<TSource>(IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            string key = GetKey(source.Expression, keyParams);
            return _cache.Get(key, source.ToArray, cacheSeconds);
        }

        public virtual List<TSource> ToCacheList<TSource>(IQueryable<TSource> source, IFunction function, params object[] keyParams)
        {
            if (function == null || function.CacheExpirationSeconds <= 0)
            {
                return source.ToList();
            }

            string key = GetKey(source.Expression, keyParams);
            return _cache.Get(key, source.ToList, function);
        }

        public virtual TSource[] ToCacheArray<TSource>(IQueryable<TSource> source, IFunction function, params object[] keyParams)
        {
            if (function == null || function.CacheExpirationSeconds <= 0)
            {
                return source.ToArray();
            }

            string key = GetKey(source.Expression, keyParams);
            return _cache.Get(key, source.ToArray, function);
        }

        public virtual PageResult<TOutputDto> ToPageCache<TEntity, TOutputDto>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            int cacheSeconds = 60,
            params object[] keyParams) where TOutputDto : IOutputDto
        {
            string key = GetKey<TEntity, TOutputDto>(source, predicate, pageCondition, keyParams);
            return _cache.Get(key, () => source.ToPage<TEntity, TOutputDto>(predicate, pageCondition), cacheSeconds);
        }

        public virtual PageResult<TOutputDto> ToPageCache<TEntity, TOutputDto>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            IFunction function,
            params object[] keyParams) where TOutputDto : IOutputDto
        {
            string key = GetKey<TEntity, TOutputDto>(source, predicate, pageCondition, keyParams);
            return _cache.Get(key, () => source.ToPage<TEntity, TOutputDto>(predicate, pageCondition), function);
        }

        public virtual List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return ToCacheList<TSource, TOutputDto>(source.Where(predicate), cacheSeconds, keyParams);
        }

        public virtual TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return ToCacheArray<TSource, TOutputDto>(source.Where(predicate), cacheSeconds, keyParams);
        }

        public virtual List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            IFunction function,
            params object[] keyParams)
        {
            return ToCacheList<TSource, TOutputDto>(source.Where(predicate), function, keyParams);
        }

        public virtual TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            IFunction function,
            params object[] keyParams)
        {
            return ToCacheArray<TSource, TOutputDto>(source.Where(predicate), function, keyParams);
        }

        public virtual List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return _cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToList(), cacheSeconds);
        }

        public virtual TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return _cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToArray(), cacheSeconds);
        }

        public virtual List<TOutputDto> ToCacheList<TSource, TOutputDto>(IQueryable<TSource> source, IFunction function, params object[] keyParams)
        {
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return _cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToList(), function);
        }

        public virtual TOutputDto[] ToCacheArray<TSource, TOutputDto>(IQueryable<TSource> source, IFunction function, params object[] keyParams)
        {
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return _cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToArray(), function);
        }

        #endregion

        #region Privates

        private static string GetKey<TEntity, TResult>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            Expression<Func<TEntity, TResult>> selector,
            params object[] keyParams)
        {
            source = source.Where(predicate);
            SortCondition[] sortConditions = pageCondition.SortConditions;
            if (sortConditions == null || sortConditions.Length == 0)
            {
                if (typeof(TEntity).IsEntityType())
                {
                    source = source.OrderBy("Id");
                }
                else if (typeof(TEntity).IsBaseOn<ICreatedTime>())
                {
                    source = source.OrderBy("CreatedTime");
                }
                else
                {
                    throw new TuanException($"Type '{typeof(TEntity)}' does not add default sorting");
                }
            }
            else
            {
                int count = 0;
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<TEntity>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<TEntity>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }

                source = orderSource;
            }

            int pageIndex = pageCondition.PageIndex, pageSize = pageCondition.PageSize;
            source = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
            IQueryable<TResult> query = source.Select(selector);
            return GetKey(query.Expression, keyParams);
        }

        private static string GetKey<TEntity, TOutputDto>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            params object[] keyParams)
            where TOutputDto : IOutputDto
        {
            source = source.Where(predicate);
            SortCondition[] sortConditions = pageCondition.SortConditions;
            if (sortConditions == null || sortConditions.Length == 0)
            {
                if (typeof(TEntity).IsEntityType())
                {
                    source = source.OrderBy("Id");
                }
                else if (typeof(TEntity).IsBaseOn<ICreatedTime>())
                {
                    source = source.OrderBy("CreatedTime");
                }
                else
                {
                    throw new TuanException($"Type '{typeof(TEntity)}' does not add default sorting");
                }
            }
            else
            {
                int count = 0;
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? CollectionPropertySorter<TEntity>.OrderBy(source, sortCondition.SortField, sortCondition.ListSortDirection)
                        : CollectionPropertySorter<TEntity>.ThenBy(orderSource, sortCondition.SortField, sortCondition.ListSortDirection);
                    count++;
                }

                source = orderSource;
            }

            int pageIndex = pageCondition.PageIndex, pageSize = pageCondition.PageSize;
            source = source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
            IQueryable<TOutputDto> query = source.ToOutput<TEntity, TOutputDto>(true);
            return GetKey(query.Expression, keyParams);
        }

        private static string GetKey<TSource, TOutputDto>(IQueryable<TSource> source,
            params object[] keyParams)
        {
            IQueryable<TOutputDto> query = source.ToOutput<TSource, TOutputDto>();
            return GetKey(query.Expression, keyParams);
        }

        private static string GetKey<TSource, TResult>(IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            params object[] keyParams)
        {
            IQueryable<TResult> query = source.Select(selector);
            return GetKey(query.Expression, keyParams);
        }

        private static string GetKey(Expression expression, params object[] keyParams)
        {
            string key;
            try
            {
                key = new ExpressionCacheKeyGenerator(expression).GetKey(keyParams);
            }
            catch (TargetInvocationException)
            {
                key = new StringCacheKeyGenerator().GetKey(keyParams);
            }

            return key.ToMd5Hash();
        }

        #endregion
    }
}