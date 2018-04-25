using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Reflection;


namespace TuanZi.EventBuses.Internal
{
    internal class EventBusBuilder : IEventBusBuilder
    {
        private readonly IAllAssemblyFinder _allAssemblyFinder;
        private readonly IEventBus _eventBus;

        public EventBusBuilder(IAllAssemblyFinder allAssemblyFinder, IEventBus eventBus)
        {
            _allAssemblyFinder = allAssemblyFinder;
            _eventBus = eventBus;
        }

        public void Build()
        {
            Assembly[] assemblies = _allAssemblyFinder.FindAll(true);
            foreach (Assembly assembly in assemblies)
            {
                _eventBus.SubscribeAll(assembly);
            }
        }
    }
}