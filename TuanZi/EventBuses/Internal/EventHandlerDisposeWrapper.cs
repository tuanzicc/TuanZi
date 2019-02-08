using System;


namespace TuanZi.EventBuses.Internal
{
    public class EventHandlerDisposeWrapper : IDisposable
    {
        private readonly Action _disposeAction;

        public EventHandlerDisposeWrapper(IEventHandler eventHandler, Action disposeAction = null)
        {
            _disposeAction = disposeAction;
            EventHandler = eventHandler;
        }

        public IEventHandler EventHandler { get; set; }

        public void Dispose()
        {
            _disposeAction?.Invoke();
        }
    }
}