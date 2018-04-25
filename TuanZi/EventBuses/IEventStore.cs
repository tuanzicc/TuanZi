using System;
using System.Collections.Generic;


namespace TuanZi.EventBuses
{
    public interface IEventStore
    {
        void Add<TEventData, TEventHandler>() where TEventData : IEventData where TEventHandler : IEventHandler, new();
        
        void Add(Type eventType, IEventHandler eventHandler);

        void Add(Type eventType, IEventHandlerFactory factory);

        void Remove<TEventData>(Action<TEventData> action) where TEventData : IEventData;
        
        void Remove(Type eventType, IEventHandler eventHandler);
        
        void RemoveAll(Type eventType);
        
        IDictionary<Type, IEventHandlerFactory[]> GetHandlers(Type eventType);
    }
}