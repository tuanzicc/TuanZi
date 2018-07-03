using System;
using System.Linq;
using System.Linq.Expressions;

using TuanZi.Entity;
using TuanZi.Extensions;
using TuanZi.Properties;


namespace TuanZi.Mapping
{
    public static class MapperExtensions
    {
        private static IMapper _mapper;

        public static void SetMapper(IMapper mapper)
        {
            mapper.CheckNotNull("mapper");
            _mapper = mapper;
        }

        public static TTarget MapTo<TTarget>(this object source)
        {
            CheckMapper();
            return _mapper.MapTo<TTarget>(source);
        }

        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            CheckMapper();
            return _mapper.MapTo(source, target);
        }

        public static IQueryable<TOutputDto> ToOutput<TOutputDto>(this IQueryable source,
            params Expression<Func<TOutputDto, object>>[] membersToExpand)
        {
            CheckMapper();
            return _mapper.ToOutput(source, membersToExpand);
        }

        private static void CheckMapper()
        {
            if (_mapper == null)
            {
                throw new NullReferenceException(Resources.Map_MapperIsNull);
            }
        }
    }
}