using System;
using System.Linq;
using System.Reflection;

using TuanZi.Core.Functions;
using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.AspNetCore.Mvc
{
    public class MvcControllerTypeFinder : FinderBase<Type>, IFunctionTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public MvcControllerTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            return assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsController()).ToArray();
        }
    }
}