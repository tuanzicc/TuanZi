using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Reflection;


namespace TuanZi.EventBuses.Internal
{
    internal class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<Type, List<IEventHandlerFactory>> _handlerFactories;

        public InMemoryEventStore()
        {
            _handlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
        }

        public void Add<TEventData, TEventHandler>() where TEventData : IEventData where TEventHandler : IEventHandler, new()
        {
            IEventHandlerFactory factory = new TransientEventHandlerFactory<TEventHandler>();
            Add(typeof(TEventData), factory);
        }

        public void Add(Type eventType, IEventHandler eventHandler)
        {
            Check.NotNull(eventType, nameof(eventType));
            Check.NotNull(eventHandler, nameof(eventHandler));

            IEventHandlerFactory factory = new SingletonEventHandlerFactory(eventHandler);
            Add(eventType, factory);
        }

        public void Add(Type eventType, IEventHandlerFactory factory)
        {
            Check.NotNull(eventType, nameof(eventType));
            Check.NotNull(factory, nameof(factory));

            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.AddIfNotExist(factory));
        }

        public void Remove<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEventData)).Locking(factories =>
            {
                factories.RemoveAll(factory =>
                {
                    if (!(factory is SingletonEventHandlerFactory singletonFactory))
                    {
                        return false;
                    }
                    if (!(singletonFactory.HandlerInstance is ActionEventHandler<TEventData> handler))
                    {
                        return false;
                    }
                    return handler.Action == action;
                });
            });
        }

        public void Remove(Type eventType, IEventHandler eventHandler)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories =>
            {
                factories.RemoveAll(factory => (factory as SingletonEventHandlerFactory)?.HandlerInstance == eventHandler);
            });
        }

        public void RemoveAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        public IDictionary<Type, IEventHandlerFactory[]> GetHandlers(Type eventType)
        {
            return _handlerFactories.Where(item => item.Key == eventType || item.Key.IsAssignableFrom(eventType))
                .ToDictionary(item => item.Key, item => item.Value.ToArray());
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return _handlerFactories.GetOrAdd(eventType, type => new List<IEventHandlerFactory>());
        }
    }
}