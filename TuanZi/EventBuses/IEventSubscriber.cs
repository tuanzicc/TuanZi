using System;
using System.Linq;
using System.Reflection;

using TuanZi.EventBuses.Internal;


namespace TuanZi.EventBuses
{
    public interface IEventSubscriber
    {
        void Subscribe<TEventData, TEventHandler>() where TEventData : IEventData where TEventHandler : IEventHandler, new();

        void Subscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        void Subscribe<TEventData>(IEventHandler eventHandler) where TEventData : IEventData;

        void Subscribe(Type eventType, IEventHandler eventHandler);

        void SubscribeAll(Assembly assembly);

        void Unsubscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        void Unsubscribe<TEventData>(IEventHandler<TEventData> eventHandler) where TEventData : IEventData;

        void Unsubscribe(Type eventType, IEventHandler eventHandler);

        void UnsubscribeAll<TEventData>() where TEventData : IEventData;

        void UnsubscribeAll(Type eventType);
    }
}