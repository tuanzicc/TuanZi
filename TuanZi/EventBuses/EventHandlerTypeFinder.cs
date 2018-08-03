using System;
using System.Linq;

using TuanZi.Finders;
using TuanZi.Reflection;


namespace TuanZi.EventBuses
{
    public class EventHandlerTypeFinder : FinderBase<Type>, IEventHandlerTypeFinder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;

        public EventHandlerTypeFinder(IAllAssemblyFinder allAssemblyFinder)
        {
            _allAssemblyFinder = allAssemblyFinder;
        }

        protected override Type[] FindAllItems()
        {
            Type baseType = typeof(IEventHandler<>);
            return _allAssemblyFinder.FindAll(true).SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDeriveClassFrom(baseType)).Distinct().ToArray();
        }
    }
}