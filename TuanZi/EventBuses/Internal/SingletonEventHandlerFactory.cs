namespace TuanZi.EventBuses.Internal
{
    internal class SingletonEventHandlerFactory : IEventHandlerFactory
    {
        public SingletonEventHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandler HandlerInstance { get; }

        public IEventHandler GetHandler()
        {
            return HandlerInstance;
        }

        public void ReleaseHandler(IEventHandler handler)
        { }
    }
}