using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.AutoMapper
{
    public class MapProfileTypeFinder : FinderBase<Type>, IMapProfileTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public MapProfileTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Profile)) && !type.IsAbstract)
                .Distinct().ToArray();
        }
    }
}