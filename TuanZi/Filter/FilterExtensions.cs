using System;
using System.Linq.Expressions;


namespace TuanZi.Filter
{
    public static class FilterExtensions
    {
        public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(this FilterGroup group)
        {
            return FilterHelper.GetExpression<TEntity>(group);
        }

        public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(this FilterRule rule)
        {
            return FilterHelper.GetExpression<TEntity>(rule);
        }
    }
}