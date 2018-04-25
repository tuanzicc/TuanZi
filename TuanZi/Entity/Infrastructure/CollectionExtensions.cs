using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using TuanZi.Collections;
using TuanZi.Filter;
using TuanZi.Mapping;
using TuanZi.Properties;


namespace TuanZi.Entity
{
    public static class CollectionExtensions
    {
        public static PageResult<TResult> ToPage<TEntity, TResult>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            Expression<Func<TEntity, TResult>> selector)
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            pageCondition.CheckNotNull("pageCondition");
            selector.CheckNotNull("selector");
            return source.ToPage(predicate,
                pageCondition.PageIndex,
                pageCondition.PageSize,
                pageCondition.SortConditions,
                selector);
        }

        public static PageResult<TResult> ToPage<TEntity, TResult>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            SortCondition[] sortConditions,
            Expression<Func<TEntity, TResult>> selector)
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            pageIndex.CheckGreaterThan("pageIndex", 0);
            pageSize.CheckGreaterThan("pageSize", 0);
            selector.CheckNotNull("selector");

            TResult[] data = source.Where(predicate, pageIndex, pageSize, out int total, sortConditions).Select(selector).ToArray();
            return new PageResult<TResult>() { Total = total, Data = data };
        }

        public static PageResult<TOutputDto> ToPage<TEntity, TOutputDto>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition)
            where TOutputDto : IOutputDto
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            pageCondition.CheckNotNull("pageCondition");
            return source.ToPage<TEntity, TOutputDto>(predicate,
                pageCondition.PageIndex,
                pageCondition.PageSize,
                pageCondition.SortConditions);
        }

        public static PageResult<TOutputDto> ToPage<TEntity, TOutputDto>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            SortCondition[] sortConditions)
            where TOutputDto : IOutputDto
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            pageIndex.CheckGreaterThan("pageIndex", 0);
            pageSize.CheckGreaterThan("pageSize", 0);

            int total;
            TOutputDto[] data = source.Where(predicate, pageIndex, pageSize, out total, sortConditions).ToOutput<TOutputDto>().ToArray();
            return new PageResult<TOutputDto>() { Total = total, Data = data };
        }

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            PageCondition pageCondition,
            out int total)
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            pageCondition.CheckNotNull("pageCondition");

            return source.Where(predicate, pageCondition.PageIndex, pageCondition.PageSize, out total, pageCondition.SortConditions);
        }

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            out int total,
            SortCondition[] sortConditions = null)
        {
            source.CheckNotNull("source");
            predicate.CheckNotNull("predicate");
            pageIndex.CheckGreaterThan("pageIndex", 0);
            pageSize.CheckGreaterThan("pageSize", 0);

            if (!typeof(TEntity).IsEntityType())
            {
                throw new InvalidOperationException(Resources.QueryCacheExtensions_TypeNotEntityType.FormatWith(typeof(TEntity).FullName));
            }

            total = source.Count(predicate);
            source = source.Where(predicate);
            if (sortConditions == null || sortConditions.Length == 0)
            {
                source = source.OrderBy("Id");
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
            return source != null
                ? source.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
        }

        public static IQueryable<TEntity> Unexpired<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class, IExpirable
        {
            DateTime now = DateTime.Now;
            Expression<Func<TEntity, bool>> predicate =
                m => (m.BeginTime == null || m.BeginTime.Value <= now) && (m.EndTime == null || m.EndTime.Value >= now);
            return source.Where(predicate);
        }

        public static IEnumerable<TEntity> Unexpired<TEntity>(this IEnumerable<TEntity> source)
            where TEntity : IExpirable
        {
            DateTime now = DateTime.Now;
            bool Func(TEntity m) => (m.BeginTime == null || m.BeginTime.Value <= now) && (m.EndTime == null || m.EndTime.Value >= now);
            return source.Where(Func);
        }

        public static IQueryable<TEntity> Expired<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class, IExpirable
        {
            DateTime now = DateTime.Now;
            Expression<Func<TEntity, bool>> predicate = m => m.EndTime != null && m.EndTime.Value < now;
            return source.Where(predicate);
        }

        public static IEnumerable<TEntity> Expired<TEntity>(this IEnumerable<TEntity> source)
            where TEntity : IExpirable
        {
            DateTime now = DateTime.Now;
            bool Func(TEntity m) => m.EndTime != null && m.EndTime.Value < now;
            return source.Where(Func);
        }
        public static IQueryable<TEntity> Unlocked<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class, ILockable
        {
            return source.Where(m => !m.IsLocked);
        }

        public static IEnumerable<TEntity> Unlocked<TEntity>(this IEnumerable<TEntity> source)
            where TEntity : ILockable
        {
            return source.Where(m => !m.IsLocked);
        }

        public static IQueryable<TEntity> Locked<TEntity>(this IQueryable<TEntity> source)
            where TEntity : class, ILockable
        {
            return source.Where(m => m.IsLocked);
        }

        public static IEnumerable<TEntity> Locked<TEntity>(this IEnumerable<TEntity> source)
            where TEntity : ILockable
        {
            return source.Where(m => m.IsLocked);
        }
    }
}