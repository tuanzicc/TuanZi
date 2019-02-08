using System;
using System.Linq.Expressions;

using TuanZi.Data;
using TuanZi.Secutiry;


namespace TuanZi.Filter
{
    public interface IFilterService
    {
        Expression<Func<T, bool>> GetExpression<T>(FilterGroup group);

        Expression<Func<T, bool>> GetExpression<T>(FilterRule rule);

        Expression<Func<T, bool>> GetDataFilterExpression<T>(FilterGroup group = null,
            DataAuthOperation operation = DataAuthOperation.Read);

        OperationResult CheckFilterGroup(FilterGroup group, Type type);
    }
}