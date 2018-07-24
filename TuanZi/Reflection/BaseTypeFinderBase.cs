using System;
using System.Linq;
using System.Reflection;

using TuanZi.Finders;


namespace TuanZi.Reflection
{
    public abstract class BaseTypeFinderBase<TBaseType> : FinderBase<Type>, ITypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        protected BaseTypeFinderBase(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDeriveClassFrom<TBaseType>()).Distinct().ToArray();
        }
    }
}