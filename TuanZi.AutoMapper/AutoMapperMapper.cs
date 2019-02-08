using System;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using TuanZi.Dependency;

using IMapper = TuanZi.Mapping.IMapper;


namespace TuanZi.AutoMapper
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class AutoMapperMapper : IMapper
    {
        public TTarget MapTo<TTarget>(object source)
        {
            return Mapper.Map<TTarget>(source);
        }

        public TTarget MapTo<TSource, TTarget>(TSource source, TTarget target)
        {
            return Mapper.Map<TSource, TTarget>(source, target);
        }

        public IQueryable<TOutputDto> ToOutput<TOutputDto>(IQueryable source, params Expression<Func<TOutputDto, object>>[] membersToExpand)
        {
            return source.ProjectTo(membersToExpand);
        }
    }
}