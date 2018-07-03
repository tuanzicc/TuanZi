using System;
using TuanZi.Dependency;

namespace TuanZi.EventBuses.Internal
{
    internal class IocEventHandlerFactory : IEventHandlerFactory
    {
        private readonly Type _handlerType;

        public IocEventHandlerFactory(Type handlerType)
        {
            _handlerType = handlerType;
        }

        public IEventHandler GetHandler()
        {
            return ServiceLocator.Instance.GetService(_handlerType) as IEventHandler;
        }

        public void ReleaseHandler(IEventHandler handler)
        { }
    }
}