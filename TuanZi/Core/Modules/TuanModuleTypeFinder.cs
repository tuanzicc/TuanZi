using System;
using System.Linq;

using TuanZi.Reflection;
using TuanZi.Finders;


namespace TuanZi.Core.Modules
{
    public class TuanModuleTypeFinder : FinderBase<Type>, ITypeFinder
    {
        public TuanModuleTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            AllAssemblyFinder = allAssemblyFinder;
        }

        public IAllAssemblyFinder AllAssemblyFinder { get; set; }

        protected override Type[] FindAllItems()
        {
            Type baseType = typeof(TuanModule);
            Type[] types = AllAssemblyFinder.FindAll(true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToArray();
            return types;
        }
    }
}