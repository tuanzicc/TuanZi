using System;
using System.Threading;
using System.Threading.Tasks;


namespace TuanZi.EventBuses.Internal
{
    internal class ActionEventHandler<TEventData> : EventHandlerBase<TEventData> where TEventData : IEventData
    {
        public ActionEventHandler(Action<TEventData> action)
        {
            Action = action;
        }

        public Action<TEventData> Action { get; }
        
        public override void Handle(TEventData eventData)
        {
            Check.NotNull(eventData, nameof(eventData));
            Action(eventData);
        }

        public override Task HandleAsync(TEventData eventData, CancellationToken cancelToken = default(CancellationToken))
        {
            Check.NotNull(eventData, nameof(eventData));
            cancelToken.ThrowIfCancellationRequested();
            return Task.Run(() => Action(eventData), cancelToken);
        }
    }
}