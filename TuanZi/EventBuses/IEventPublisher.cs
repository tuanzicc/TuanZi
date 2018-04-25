using System;
using System.Threading.Tasks;


namespace TuanZi.EventBuses
{
    public interface IEventPublisher
    {
        void Publish<TEventData>(TEventData eventData) where TEventData : IEventData;

        void Publish<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;

        void Publish(Type eventType, IEventData eventData);

        void Publish(Type eventType, object eventSource, IEventData eventData);

        Task PublishAsync<TEventData>(TEventData eventData) where TEventData : IEventData;

        Task PublishAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;

        Task PublishAsync(Type eventType, IEventData eventData);

        Task PublishAsync(Type eventType, object eventSource, IEventData eventData);
    }
}