using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Collections;
using TuanZi.Core.Functions;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Entity;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Mapping;
using TuanZi.Properties;
using TuanZi.Reflection;

namespace TuanZi.Caching
{
   
    public static class DistributedCacheExtensions
    {
        public static void Set(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options = null)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            string json = value.ToJsonString();
            if (options == null)
            {
                cache.SetString(key, json);
            }
            else
            {
                cache.SetString(key, json, options);
            }
        }

        public static async Task SetAsync(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options = null)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            string json = value.ToJsonString();
            if (options == null)
            {
                await cache.SetStringAsync(key, json);
            }
            else
            {
                await cache.SetStringAsync(key, json, options);
            }
        }

        public static void Set(this IDistributedCache cache, string key, object value, int cacheSeconds)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            cache.Set(key, value, options);
        }

        public static Task SetAsync(this IDistributedCache cache, string key, object value, int cacheSeconds)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.SetAsync(key, value, options);
        }

        public static void Set(this IDistributedCache cache, string key, object value, IFunction function)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.NotNull(function, nameof(function));

            DistributedCacheEntryOptions options = function.ToCacheOptions();
            if (options == null)
            {
                return;
            }
            cache.Set(key, value, options);
        }

        public static Task SetAsync(this IDistributedCache cache, string key, object value, IFunction function)
        {
            Check.NotNullOrEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));
            Check.NotNull(function, nameof(function));

            DistributedCacheEntryOptions options = function.ToCacheOptions();
            if (options == null)
            {
                return Task.FromResult(0);
            }
            return cache.SetAsync(key, value, options);
        }

        public static TResult Get<TResult>(this IDistributedCache cache, string key)
        {
            string json = cache.GetString(key);
            if (json == null)
            {
                return default(TResult);
            }
            return json.FromJsonString<TResult>();
        }

        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key)
        {
            string json = await cache.GetStringAsync(key);
            if (json == null)
            {
                return default(TResult);
            }
            return json.FromJsonString<TResult>();
        }

        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, DistributedCacheEntryOptions options = null)
        {
            TResult result = cache.Get<TResult>(key);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = getFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            cache.Set(key, result, options);
            return result;
        }

        public static async Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, DistributedCacheEntryOptions options = null)
        {
            TResult result = await cache.GetAsync<TResult>(key);
            if (!Equals(result, default(TResult)))
            {
                return result;
            }
            result = await getAsyncFunc();
            if (Equals(result, default(TResult)))
            {
                return default(TResult);
            }
            await cache.SetAsync(key, result, options);
            return result;
        }

        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, int cacheSeconds)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.Get<TResult>(key, getFunc, options);
        }

        public static Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, int cacheSeconds)
        {
            Check.GreaterThan(cacheSeconds, nameof(cacheSeconds), 0);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheSeconds));
            return cache.GetAsync<TResult>(key, getAsyncFunc, options);
        }

        public static TResult Get<TResult>(this IDistributedCache cache, string key, Func<TResult> getFunc, IFunction function)
        {
            DistributedCacheEntryOptions options = function.ToCacheOptions();
            if (options == null)
            {
                return getFunc();
            }
            return cache.Get<TResult>(key, getFunc, options);
        }

        public static Task<TResult> GetAsync<TResult>(this IDistributedCache cache, string key, Func<Task<TResult>> getAsyncFunc, IFunction function)
        {
            DistributedCacheEntryOptions options = function.ToCacheOptions();
            if (options == null)
            {
                return getAsyncFunc();
            }
            return cache.GetAsync<TResult>(key, getAsyncFunc, options);
        }

        public static PageResult<TResult> ToPageCache<TEntity, TResult>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> pridicate,
            PageCondition pageCondition,
            Expression<Func<TEntity, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, pridicate, pageCondition, selector, keyParams);
            return cache.Get(key, () => source.ToPage(pridicate, pageCondition, selector), cacheSeconds);
        }

        public static PageResult<TResult> ToPageCache<TEntity, TResult>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> pridicate,
            PageCondition pageCondition,
            Expression<Func<TEntity, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, pridicate, pageCondition, selector, keyParams);
            return cache.Get(key, () => source.ToPage(pridicate, pageCondition, selector), function);
        }

        public static List<TResult> ToCacheList<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheList(selector, cacheSeconds, keyParams);
        }

        public static TResult[] ToCacheArray<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheArray(selector, cacheSeconds, keyParams);
        }

        public static List<TResult> ToCacheList<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheList(selector, function, keyParams);
        }

        public static TResult[] ToCacheArray<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheArray(selector, function, keyParams);
        }

        public static List<TResult> ToCacheList<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, selector, keyParams);
            return cache.Get(key, () => source.Select(selector).ToList(), cacheSeconds);
        }

        public static TResult[] ToCacheArray<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, selector, keyParams);
            return cache.Get(key, () => source.Select(selector).ToArray(), cacheSeconds);
        }

        public static List<TResult> ToCacheList<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, selector, keyParams);
            return cache.Get(key, () => source.Select(selector).ToList(), function);
        }

        public static TResult[] ToCacheArray<TSource, TResult>(this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            IFunction function,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source, selector, keyParams);
            return cache.Get(key, () => source.Select(selector).ToArray(), function);
        }

        public static List<TSource> ToCacheList<TSource>(this IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source.Expression, keyParams);
            return cache.Get(key, source.ToList, cacheSeconds);
        }

        public static TSource[] ToCacheArray<TSource>(this IQueryable<TSource> source, int cacheSeconds = 60, params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source.Expression, keyParams);
            return cache.Get(key, source.ToArray, cacheSeconds);
        }

        public static List<TSource> ToCacheList<TSource>(this IQueryable<TSource> source, IFunction function, params object[] keyParams)
        {
            if (function == null || function.CacheExpirationSeconds <= 0)
            {
                return source.ToList();
            }
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source.Expression, keyParams);
            return cache.Get(key, source.ToList, function);
        }

        public static TSource[] ToCacheArray<TSource>(this IQueryable<TSource> source, IFunction function, params object[] keyParams)
        {
            if (function == null || function.CacheExpirationSeconds <= 0)
            {
                return source.ToArray();
            }
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey(source.Expression, keyParams);
            return cache.Get(key, source.ToArray, function);
        }

        #region OutputDto

        public static PageResult<TOutputDto> ToPageCache<TEntity, TOutputDto>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            int cacheSeconds = 60, params object[] keyParams)
            where TOutputDto : IOutputDto
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey<TEntity, TOutputDto>(source, predicate, pageCondition, keyParams);
            return cache.Get(key, () => source.ToPage<TEntity, TOutputDto>(predicate, pageCondition), cacheSeconds);
        }

        public static PageResult<TOutputDto> ToPageCache<TEntity, TOutputDto>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            IFunction function, params object[] keyParams)
            where TOutputDto : IOutputDto
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey<TEntity, TOutputDto>(source, predicate, pageCondition, keyParams);
            return cache.Get(key, () => source.ToPage<TEntity, TOutputDto>(predicate, pageCondition), function);
        }

        public static List<TOutputDto> ToCacheList<TSource, TOutputDto>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheList<TSource, TOutputDto>(cacheSeconds, keyParams);
        }

        public static TOutputDto[] ToCacheArray<TSource, TOutputDto>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheArray<TSource, TOutputDto>(cacheSeconds, keyParams);
        }

        public static List<TOutputDto> ToCacheList<TSource, TOutputDto>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            IFunction function,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheList<TSource, TOutputDto>(function, keyParams);
        }

        public static TOutputDto[] ToCacheArray<TSource, TOutputDto>(this IQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            IFunction function,
            params object[] keyParams)
        {
            return source.Where(predicate).ToCacheArray<TSource, TOutputDto>(function, keyParams);
        }

        public static List<TOutputDto> ToCacheList<TSource, TOutputDto>(this IQueryable<TSource> source,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToList(), cacheSeconds);
        }

        public static TOutputDto[] ToCacheArray<TSource, TOutputDto>(this IQueryable<TSource> source,
            int cacheSeconds = 60,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToArray(), cacheSeconds);
        }

        public static List<TOutputDto> ToCacheList<TSource, TOutputDto>(this IQueryable<TSource> source,
            IFunction function,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToList(), function);
        }

        public static TOutputDto[] ToCacheArray<TSource, TOutputDto>(this IQueryable<TSource> source,
            IFunction function,
            params object[] keyParams)
        {
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            string key = GetKey<TSource, TOutputDto>(source, keyParams);
            return cache.Get(key, () => source.ToOutput<TSource, TOutputDto>().ToArray(), function);
        }

        #endregion

        public static DistributedCacheEntryOptions ToCacheOptions(this IFunction function)
        {
            Check.NotNull(function, nameof(function));
            if (function.CacheExpirationSeconds == 0)
            {
                return null;
            }
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            if (!function.IsCacheSliding)
            {
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(function.CacheExpirationSeconds));
            }
            else
            {
                options.SetSlidingExpiration(TimeSpan.FromSeconds(function.CacheExpirationSeconds));
            }
            return options;
        }

        private static string GetKey<TEntity, TResult>(IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            Expression<Func<TEntity, TResult>> selector, params object[] keyParams)
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
                    throw new TuanException($"Type '{typeof(TEntity)}' did not add default sorting");
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
            IQueryable<TOutputDto> query = source.ToOutput<TEntity, TOutputDto>();
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
    }
}