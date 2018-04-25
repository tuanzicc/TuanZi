using System;
using System.Linq;
using System.Linq.Expressions;


namespace TuanZi.Mapping
{
    public interface IMapper
    {
        TTarget MapTo<TTarget>(object source);

        TTarget MapTo<TSource, TTarget>(TSource source, TTarget target);

        IQueryable<TOutputDto> ToOutput<TOutputDto>(IQueryable source, params Expression<Func<TOutputDto, object>>[] membersToExpand);
    }
}