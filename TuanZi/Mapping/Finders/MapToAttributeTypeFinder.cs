using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Mapping
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class MapToAttributeTypeFinder : AttributeTypeFinderBase<MapToAttribute>, IMapToAttributeTypeFinder
    {
        public MapToAttributeTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        { }
    }
}