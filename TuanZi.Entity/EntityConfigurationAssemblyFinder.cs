using System;
using System.Linq;
using System.Reflection;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Entity
{
    public class EntityConfigurationAssemblyFinder : FinderBase<Assembly>, IEntityConfigurationAssemblyFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public EntityConfigurationAssemblyFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Assembly[] FindAllItems()
        {
            Type baseType = typeof(IEntityRegister);
            Assembly[] assemblies = _allAssemblyFinder.Find(assembly => assembly.GetTypes()
                .Any(type => baseType.IsAssignableFrom(type) && !type.IsAbstract));
            return assemblies;
        }
    }
}