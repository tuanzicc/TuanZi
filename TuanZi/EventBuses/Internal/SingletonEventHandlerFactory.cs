namespace TuanZi.EventBuses.Internal
{
    internal class SingletonEventHandlerFactory : IEventHandlerFactory
    {
        public SingletonEventHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandler HandlerInstance { get; }

        public EventHandlerDisposeWrapper GetHandler()
        {
            return new EventHandlerDisposeWrapper(HandlerInstance);
        }
    }
}