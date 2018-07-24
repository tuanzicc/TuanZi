using System;
using System.Linq;
using System.Reflection;

using TuanZi.Finders;


namespace TuanZi.Reflection
{
    public class AttributeTypeFinderBase<TAttributeType> : FinderBase<Type>, ITypeFinder
        where TAttributeType : Attribute
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public AttributeTypeFinderBase(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.HasAttribute<TAttributeType>()).Distinct().ToArray();
        }
    }
}