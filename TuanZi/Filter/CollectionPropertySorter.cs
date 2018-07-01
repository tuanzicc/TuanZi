
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Properties;


namespace TuanZi.Filter
{
    public static class CollectionPropertySorter<T>
    {
        private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new ConcurrentDictionary<string, LambdaExpression>();

        public static IOrderedEnumerable<T> OrderBy(IEnumerable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName" );
            dynamic expression = GetKeySelector(propertyName);
            dynamic keySelector = expression.Compile();
            return sortDirection == ListSortDirection.Ascending
                ? Enumerable.OrderBy(source, keySelector)
                : Enumerable.OrderByDescending(source, keySelector);
        }

        public static IOrderedEnumerable<T> ThenBy(IOrderedEnumerable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName" );
            dynamic expression = GetKeySelector(propertyName);
            dynamic keySelector = expression.Compile();
            return sortDirection == ListSortDirection.Ascending
                ? Enumerable.ThenBy(source, keySelector)
                : Enumerable.ThenByDescending(source, keySelector);
        }

        public static IOrderedQueryable<T> OrderBy(IQueryable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName" );
            dynamic keySelector = GetKeySelector(propertyName);
            return sortDirection == ListSortDirection.Ascending
                ? Queryable.OrderBy(source, keySelector)
                : Queryable.OrderByDescending(source, keySelector);
        }

        public static IOrderedQueryable<T> ThenBy(IOrderedQueryable<T> source, string propertyName, ListSortDirection sortDirection)
        {
            propertyName.CheckNotNullOrEmpty("propertyName" );
            dynamic keySelector = GetKeySelector(propertyName);
            return sortDirection == ListSortDirection.Ascending
                ? Queryable.ThenBy(source, keySelector)
                : Queryable.ThenByDescending(source, keySelector);
        }

        private static LambdaExpression GetKeySelector(string keyName)
        {
            Type type = typeof(T);
            string key = type.FullName + "." + keyName;
            if (Cache.ContainsKey(key))
            {
                return Cache[key];
            }
            ParameterExpression param = Expression.Parameter(type);
            string[] propertyNames = keyName.Split('.');
            Expression propertyAccess = param;
            foreach (string propertyName in propertyNames)
            {
                PropertyInfo property = type.GetProperty(propertyName);
                if (property == null)
                {
                    throw new TuanException(string.Format(Resources.ObjectExtensions_PropertyNameNotExistsInType, propertyName));
                }
                type = property.PropertyType;
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
            LambdaExpression keySelector = Expression.Lambda(propertyAccess, param);
            Cache[key] = keySelector;
            return keySelector;
        }
    }
}