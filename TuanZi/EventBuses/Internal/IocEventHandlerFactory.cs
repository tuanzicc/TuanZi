using System;

using Microsoft.Extensions.DependencyInjection;

using TuanZi.Dependency;


namespace TuanZi.EventBuses.Internal
{
    internal class IocEventHandlerFactory : IEventHandlerFactory
    {
        private readonly IHybridServiceScopeFactory _serviceScopeFactory;
        private readonly Type _handlerType;

        public IocEventHandlerFactory(IHybridServiceScopeFactory serviceScopeFactory, Type handlerType)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _handlerType = handlerType;
        }

        public EventHandlerDisposeWrapper GetHandler()
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            return new EventHandlerDisposeWrapper((IEventHandler)scope.ServiceProvider.GetService(_handlerType), () => scope.Dispose());
        }
    }
}