﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using TuanZi.Exceptions;
using TuanZi.EventBuses.Internal;
using TuanZi.Reflection;
using TuanZi.Data;
using TuanZi.Extensions;

namespace TuanZi.EventBuses
{
    public abstract class EventBusBase : IEventBus
    {
        protected readonly IEventStore _EventStore;
        protected readonly ILogger _Logger;

        protected EventBusBase(IEventStore eventStore, ILogger logger)
        {
            _EventStore = eventStore;
            _Logger = logger;
        }

        #region Implementation of IEventSubscriber

        public virtual void Subscribe<TEventData, TEventHandler>() where TEventData : IEventData where TEventHandler : IEventHandler, new()
        {
            _EventStore.Add<TEventData, TEventHandler>();
        }

        public virtual void Subscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            IEventHandler eventHandler = new ActionEventHandler<TEventData>(action);
            Subscribe<TEventData>(eventHandler);
        }

        public virtual void Subscribe<TEventData>(IEventHandler eventHandler) where TEventData : IEventData
        {
            Check.NotNull(eventHandler, nameof(eventHandler));

            Subscribe(typeof(TEventData), eventHandler);
        }

        public virtual void Subscribe(Type eventType, IEventHandler eventHandler)
        {
            Check.NotNull(eventType, nameof(eventType));
            Check.NotNull(eventHandler, nameof(eventHandler));

            _EventStore.Add(eventType, eventHandler);
        }

        public virtual void SubscribeAll(Assembly assembly)
        {
            assembly.CheckNotNull("assembly");

            Type[] handlerTypes = assembly.GetTypes().Where(type => type.IsDeriveClassFrom(typeof(IEventHandler<>))).ToArray();
            if (handlerTypes.Length == 0)
            {
                return;
            }
            foreach (Type handlerType in handlerTypes)
            {
                Type handlerInterface = handlerType.GetInterface("IEventHandler`1");
                if (handlerInterface == null)
                {
                    continue;
                }
                Type eventType = handlerInterface.GetGenericArguments()[0];
                IEventHandlerFactory factory = new IocEventHandlerFactory(handlerType);
                _EventStore.Add(eventType, factory);
            }
        }

        public virtual void Unsubscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            _EventStore.Remove(action);
        }

        public virtual void Unsubscribe<TEventData>(IEventHandler<TEventData> eventHandler) where TEventData : IEventData
        {
            Check.NotNull(eventHandler, nameof(eventHandler));

            Unsubscribe(typeof(TEventData), eventHandler);
        }

        public virtual void Unsubscribe(Type eventType, IEventHandler eventHandler)
        {
            _EventStore.Remove(eventType, eventHandler);
        }

        public virtual void UnsubscribeAll<TEventData>() where TEventData : IEventData
        {
            UnsubscribeAll(typeof(TEventData));
        }

        public virtual void UnsubscribeAll(Type eventType)
        {
            _EventStore.RemoveAll(eventType);
        }

        #endregion

        #region Implementation of IEventPublisher

        public virtual void PublishSync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            PublishSync<TEventData>(null, eventData);
        }

        public virtual void PublishSync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            PublishSync(typeof(TEventData), eventSource, eventData);
        }

        public virtual void PublishSync(Type eventType, IEventData eventData)
        {
            PublishSync(eventType, null, eventData);
        }

        public virtual void PublishSync(Type eventType, object eventSource, IEventData eventData)
        {
            eventData.EventSource = eventSource;

            IDictionary<Type, IEventHandlerFactory[]> dict = _EventStore.GetHandlers(eventType);
            foreach (var typeItem in dict)
            {
                foreach (IEventHandlerFactory factory in typeItem.Value)
                {
                    InvokeHandler(factory, eventType, eventData);
                }
            }
        }

        public virtual Task PublishAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return PublishAsync<TEventData>(null, eventData);
        }

        public virtual Task PublishAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return PublishAsync(typeof(TEventData), eventSource, eventData);
        }

        public virtual Task PublishAsync(Type eventType, IEventData eventData)
        {
            return PublishAsync(eventType, null, eventData);
        }

        public virtual async Task PublishAsync(Type eventType, object eventSource, IEventData eventData)
        {
            eventData.EventSource = eventSource;

            IDictionary<Type, IEventHandlerFactory[]> dict = _EventStore.GetHandlers(eventType);
            foreach (var typeItem in dict)
            {
                foreach (IEventHandlerFactory factory in typeItem.Value)
                {
                    await InvokeHandlerAsync(factory, eventType, eventData);
                }
            }
        }

        protected void InvokeHandler(IEventHandlerFactory factory, Type eventType, IEventData eventData)
        {
            IEventHandler handler = factory.GetHandler();
            if (handler == null)
            {
                throw new TuanException($"Event handler for event '{eventData.GetType()}' cannot be found");
            }
            if (!handler.CanHandle(eventData))
            {
                return;
            }
            try
            {
                handler.Handle(eventData);
            }
            catch (Exception ex)
            {
                string msg = $"Exception thrown when executing handler '{handler.GetType()}'{0}' for event '{eventType.Name}'{0}': {ex.Message}";
                _Logger.LogError(ex, msg);
            }
            finally
            {
                factory.ReleaseHandler(handler);
            }
        }

        protected virtual Task InvokeHandlerAsync(IEventHandlerFactory factory, Type eventType, IEventData eventData)
        {
            IEventHandler handler = factory.GetHandler();
            if (!handler.CanHandle(eventData))
            {
                return Task.FromResult(0);
            }
            Task.Run(async () =>
            {
                try
                {
                    await handler.HandleAsync(eventData);
                }
                catch (Exception ex)
                {
                    string msg = $"Exception thrown when executing handler '{handler.GetType()}'{0}' for event '{eventType.Name}'{0}': {ex.Message}";
                    _Logger.LogError(ex, msg);
                }
                finally
                {
                    factory.ReleaseHandler(handler);
                }
            });
            return Task.FromResult(0);
        }

        #endregion
    }

}