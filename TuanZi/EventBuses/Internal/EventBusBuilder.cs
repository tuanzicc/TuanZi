using System;
using System.Reflection;

using TuanZi.Dependency;
using TuanZi.Reflection;


namespace TuanZi.EventBuses.Internal
{

    internal class EventBusBuilder : IEventBusBuilder
    {
        private readonly IEventHandlerTypeFinder _typeFinder;
        private readonly IEventBus _eventBus;

        public EventBusBuilder(IEventHandlerTypeFinder typeFinder, IEventBus eventBus)
        {
            _typeFinder = typeFinder;
            _eventBus = eventBus;
        }

        public void Build()
        {
            Type[] types = _typeFinder.FindAll(true);
            if (types.Length == 0)
            {
                return;
            }
            _eventBus.SubscribeAll(types);
        }
    }
}