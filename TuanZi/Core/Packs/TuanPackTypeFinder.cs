using System;
using System.Linq;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Core.Packs
{
    public class TuanPackTypeFinder : FinderBase<Type>, ITypeFinder
    {
        public TuanPackTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            AllAssemblyFinder = allAssemblyFinder;
        }

        public IAllAssemblyFinder AllAssemblyFinder { get; set; }

        protected override Type[] FindAllItems()
        {
            Type baseType = typeof(TuanPack);
            Type[] types = AllAssemblyFinder.FindAll(true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToArray();
            return types;
        }
    }
}