using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Dependency
{
    public class SingletonDependencyTypeFinder : FinderBase<Type>, ITypeFinder
    {
        public SingletonDependencyTypeFinder()
        {
            AllAssemblyFinder = new AppDomainAllAssemblyFinder();
        }

        public IAllAssemblyFinder AllAssemblyFinder { get; set; }

        protected override Type[] FindAllItems()
        {
            Type baseType = typeof(ISingletonDependency);
            Type[] types = AllAssemblyFinder.FindAll(fromCache: true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && baseType.IsAssignableFrom(type) && !type.HasAttribute<IgnoreDependencyAttribute>() && !type.IsAbstract)
                .ToArray();
            return types;
        }
    }
}