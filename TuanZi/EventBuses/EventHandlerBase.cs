using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.EventBuses
{
    public abstract class EventHandlerBase<TEventData> : IEventHandler<TEventData> where TEventData : IEventData
    {
        public virtual bool CanHandle(IEventData eventData)
        {
            return eventData.GetType() == typeof(TEventData);
        }

        public virtual void Handle(IEventData eventData)
        {
            if (!CanHandle(eventData))
            {
                return;
            }
            Handle((TEventData)eventData);
        }

        public virtual Task HandleAsync(IEventData eventData, CancellationToken cancelToken = default(CancellationToken))
        {
            if (!CanHandle(eventData))
            {
                return Task.FromResult(0);
            }
            return HandleAsync((TEventData)eventData, cancelToken);
        }

        public abstract void Handle(TEventData eventData);

        public virtual Task HandleAsync(TEventData eventData, CancellationToken cancelToken = default(CancellationToken))
        {
            throw new NotSupportedException("Current event handler does not support asynchronous event handling");
        }
    }
}
