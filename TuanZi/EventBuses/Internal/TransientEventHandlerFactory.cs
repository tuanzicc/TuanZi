using System;


namespace TuanZi.EventBuses.Internal
{
    internal class TransientEventHandlerFactory<TEventHandler> : IEventHandlerFactory
        where TEventHandler : IEventHandler, new()
    {
        public EventHandlerDisposeWrapper GetHandler()
        {
            IEventHandler handler = new TEventHandler();
            return new EventHandlerDisposeWrapper(handler, () => (handler as IDisposable)?.Dispose());
        }
    }
}