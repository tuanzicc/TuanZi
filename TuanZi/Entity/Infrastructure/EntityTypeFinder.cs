using System;
using System.Linq;
using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Finders;
using TuanZi.Mapping;
using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public class EntityTypeFinder : FinderBase<Type>, IEntityTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public EntityTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Type baseType = typeof(IEntity<>);
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDeriveClassFrom(baseType)).Distinct().ToArray();
        }
    }
}