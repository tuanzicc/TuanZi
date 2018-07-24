using System;
using System.Threading.Tasks;


namespace TuanZi.EventBuses
{
    public interface IEventPublisher
    {
        void Publish<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData;

        void Publish<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData;

        void Publish(Type eventType, IEventData eventData, bool wait = true);

        void Publish(Type eventType, object eventSource, IEventData eventData, bool wait = true);

        Task PublishAsync<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData;

        Task PublishAsync<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData;

        Task PublishAsync(Type eventType, IEventData eventData, bool wait = true);

        Task PublishAsync(Type eventType, object eventSource, IEventData eventData, bool wait = true);
    }
}