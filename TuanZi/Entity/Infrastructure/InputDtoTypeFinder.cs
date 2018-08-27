using System;
using System.Linq;
using System.Reflection;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.Entity.Infrastructure
{
    public class InputDtoTypeFinder : FinderBase<Type>, IInputDtoTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public InputDtoTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDeriveClassFrom(typeof(IInputDto<>))).Distinct().ToArray();
        }
    }
}