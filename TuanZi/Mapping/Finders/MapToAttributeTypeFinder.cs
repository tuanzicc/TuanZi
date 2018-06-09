using System;
using System.Linq;
using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Mapping
{
    public class MapToAttributeTypeFinder : FinderBase<Type>, IMapToAttributeTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public MapToAttributeTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && type.HasAttribute<MapToAttribute>() && !type.IsAbstract)
                .Distinct().ToArray();
        }
    }
}