using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Dependency
{
    public class TransientDependencyTypeFinder : FinderBase<Type>, ITypeFinder
    {
        public TransientDependencyTypeFinder()
        {
            AllAssemblyFinder = new AppDomainAllAssemblyFinder();
        }

        public IAllAssemblyFinder AllAssemblyFinder { get; set; }

        protected override Type[] FindAllItems()
        {
            Type baseType = typeof(ITransientDependency);
            Type[] types = AllAssemblyFinder.FindAll(fromCache:true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && !type.HasAttribute<IgnoreDependencyAttribute>() && !type.IsAbstract && !type.IsInterface)
                .ToArray();
            return types;
        }
    }
}