using System;
using System.Linq;
using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Mapping
{
    public class MapFromAttributeTypeFinder : FinderBase<Type>, IMapFromAttributeTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public MapFromAttributeTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && type.HasAttribute<MapFromAttribute>() && !type.IsAbstract)
                .Distinct().ToArray();
        }
    }
}