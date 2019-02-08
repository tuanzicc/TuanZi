using Microsoft.Extensions.DependencyInjection;


using TuanZi.Dependency;
using TuanZi.Reflection;


namespace TuanZi.Mapping
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class MapFromAttributeTypeFinder : AttributeTypeFinderBase<MapFromAttribute>, IMapFromAttributeTypeFinder
    {
        public MapFromAttributeTypeFinder(IAllAssemblyFinder allAssemblyFinder)
            : base(allAssemblyFinder)
        { }
    }
}