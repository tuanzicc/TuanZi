using System;


namespace TuanZi.EventBuses.Internal
{
    internal class TransientEventHandlerFactory<TEventHandler> : IEventHandlerFactory
        where TEventHandler : IEventHandler, new()
    {
        public IEventHandler GetHandler()
        {
            return new TEventHandler();
        }

        public void ReleaseHandler(IEventHandler handler)
        {
            (handler as IDisposable)?.Dispose();
        }
    }
}