using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using TuanZi.Collections;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Filter;
using TuanZi.Mapping;
using TuanZi.Properties;
using TuanZi.Reflection;
using TuanZi.Secutiry;

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

            return source.ToPage(predicate, pageCondition.PageIndex, pageCondition.PageSize, pageCondition.SortConditions, selector);
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

            TOutputDto[] data = source.Where(predicate, pageIndex, pageSize, out int total, sortConditions).ToOutput<TEntity, TOutputDto>().ToArray();
            return new PageResult<TOutputDto>() { Total = total, Data = data };
        }

        public static IQueryable<TOutputDto> ToOutput<TEntity, TOutputDto>(this IQueryable<TEntity> source, bool getKey = false)
        {
            if (!typeof(TOutputDto).IsBaseOn<IDataAuthEnabled>() || getKey)
            {
                return MapperExtensions.ToOutput<TEntity, TOutputDto>(source);
            }

            List<TEntity> entities = source.ToList();
            List<TOutputDto> dtos = new List<TOutputDto>();
            Func<TEntity, bool> updateFunc = FilterHelper.GetDataFilterExpression<TEntity>(null, DataAuthOperation.Update).Compile();
            Func<TEntity, bool> deleteFunc = FilterHelper.GetDataFilterExpression<TEntity>(null, DataAuthOperation.Delete).Compile();
            foreach (TEntity entity in entities)
            {
                TOutputDto dto = entity.MapTo<TOutputDto>();
                IDataAuthEnabled dto2 = (IDataAuthEnabled)dto;
                dto2.Updatable = updateFunc(entity);
                dto2.Deletable = deleteFunc(entity);
                dtos.Add(dto);
            }
            return dtos.AsQueryable();
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

            total = source.Count(predicate);
            source = source.Where(predicate);
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