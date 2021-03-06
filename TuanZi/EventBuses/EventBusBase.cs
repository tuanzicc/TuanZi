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
using TuanZi.Dependency;
using Microsoft.Extensions.DependencyInjection;

namespace TuanZi.EventBuses
{

    public abstract class EventBusBase : IEventBus
    {
        protected EventBusBase(IHybridServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider)
        {
            ServiceScopeFactory = serviceScopeFactory;
            EventStore = serviceProvider.GetService<IEventStore>();
            Logger = serviceProvider.GetLogger(GetType());
        }

        protected IHybridServiceScopeFactory ServiceScopeFactory { get; }

        protected IEventStore EventStore { get; }

        protected ILogger Logger { get; }

        #region Implementation of IEventSubscriber

        public virtual void Subscribe<TEventData, TEventHandler>() where TEventData : IEventData where TEventHandler : IEventHandler, new()
        {
            EventStore.Add<TEventData, TEventHandler>();
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

            EventStore.Add(eventType, eventHandler);
        }

        public virtual void SubscribeAll(Type[] eventHandlerTypes)
        {
            Check.NotNull(eventHandlerTypes, nameof(eventHandlerTypes));

            foreach (Type eventHandlerType in eventHandlerTypes)
            {
                Type handlerInterface = eventHandlerType.GetInterface("IEventHandler`1");
                if (handlerInterface == null)
                {
                    continue;
                }
                Type eventDataType = handlerInterface.GetGenericArguments()[0];
                IEventHandlerFactory factory = new IocEventHandlerFactory(ServiceScopeFactory, eventHandlerType);
                EventStore.Add(eventDataType, factory);
                Logger.LogDebug($"Create a subscription pairing of event '{eventDataType}' to processor '{eventHandlerType}'");
            }
            Logger.LogInformation($"creates event subscriptions for {eventHandlerTypes.Length} event handlers from the assembly");
        }

        public virtual void Unsubscribe<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            Check.NotNull(action, nameof(action));

            EventStore.Remove(action);
        }

        public virtual void Unsubscribe<TEventData>(IEventHandler<TEventData> eventHandler) where TEventData : IEventData
        {
            Check.NotNull(eventHandler, nameof(eventHandler));

            Unsubscribe(typeof(TEventData), eventHandler);
        }

        public virtual void Unsubscribe(Type eventType, IEventHandler eventHandler)
        {
            EventStore.Remove(eventType, eventHandler);
        }

        public virtual void UnsubscribeAll<TEventData>() where TEventData : IEventData
        {
            UnsubscribeAll(typeof(TEventData));
        }

        public virtual void UnsubscribeAll(Type eventType)
        {
            EventStore.RemoveAll(eventType);
        }

        #endregion

        #region Implementation of IEventPublisher

        public virtual void Publish<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            Publish<TEventData>(null, eventData, wait);
        }

        public virtual void Publish<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            Publish(typeof(TEventData), eventSource, eventData, wait);
        }

        public virtual void Publish(Type eventType, IEventData eventData, bool wait = true)
        {
            Publish(eventType, null, eventData, wait);
        }

        public virtual void Publish(Type eventType, object eventSource, IEventData eventData, bool wait = true)
        {
            eventData.EventSource = eventSource;

            IDictionary<Type, IEventHandlerFactory[]> dict = EventStore.GetHandlers(eventType);
            foreach (var typeItem in dict)
            {
                foreach (IEventHandlerFactory factory in typeItem.Value)
                {
                    InvokeHandler(factory, eventType, eventData, wait);
                }
            }
        }

        public virtual Task PublishAsync<TEventData>(TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            return PublishAsync<TEventData>(null, eventData, wait);
        }

        public virtual Task PublishAsync<TEventData>(object eventSource, TEventData eventData, bool wait = true) where TEventData : IEventData
        {
            return PublishAsync(typeof(TEventData), eventSource, eventData, wait);
        }

        public virtual Task PublishAsync(Type eventType, IEventData eventData, bool wait = true)
        {
            return PublishAsync(eventType, null, eventData, wait);
        }

        public virtual async Task PublishAsync(Type eventType, object eventSource, IEventData eventData, bool wait = true)
        {
            eventData.EventSource = eventSource;

            IDictionary<Type, IEventHandlerFactory[]> dict = EventStore.GetHandlers(eventType);
            foreach (var typeItem in dict)
            {
                foreach (IEventHandlerFactory factory in typeItem.Value)
                {
                    await InvokeHandlerAsync(factory, eventType, eventData, wait);
                }
            }
        }

        protected void InvokeHandler(IEventHandlerFactory factory, Type eventType, IEventData eventData, bool wait = true)
        {
            EventHandlerDisposeWrapper handlerWrapper = factory.GetHandler();
            IEventHandler handler = handlerWrapper.EventHandler;
            try
            {
                if (handler == null)
                {
                    Logger.LogWarning($"Event handler for event source '{eventData.GetType()}' could not be found");
                    return;
                }
                if (!handler.CanHandle(eventData))
                {
                    return;
                }
                if (wait)
                {
                    Run(factory, handler, eventType, eventData);
                }
                else
                {
                    Task.Run(() =>
                    {
                        Run(factory, handler, eventType, eventData);
                    });
                }
            }
            finally
            {
                handlerWrapper.Dispose();
            }
        }

        protected virtual Task InvokeHandlerAsync(IEventHandlerFactory factory, Type eventType, IEventData eventData, bool wait = true)
        {
            EventHandlerDisposeWrapper handlerWrapper = factory.GetHandler();
            IEventHandler handler = handlerWrapper.EventHandler;
            try
            {
                if (handler == null)
                {
                    Logger.LogWarning($"Event handler for event source '{eventData.GetType()}' could not be found");
                    return Task.FromResult(0);
                }
                if (!handler.CanHandle(eventData))
                {
                    return Task.FromResult(0);
                }
                if (wait)
                {
                    return RunAsync(factory, handler, eventType, eventData);
                }
                Task.Run(async () =>
                {
                    await RunAsync(factory, handler, eventType, eventData);
                });
                return Task.FromResult(0);
            }
            finally
            {
                handlerWrapper.Dispose();
            }
        }

        private void Run(IEventHandlerFactory factory, IEventHandler handler, Type eventType, IEventData eventData)
        {
            try
            {
                handler.Handle(eventData);
            }
            catch (Exception ex)
            {
                string msg = $"Exception thrown when executing handler '{handler.GetType()}'{0}' for event '{eventType.Name}'{0}': {ex.Message}";
                Logger.LogError(ex, msg);
            }
        }

        private Task RunAsync(IEventHandlerFactory factory, IEventHandler handler, Type eventType, IEventData eventData)
        {
            try
            {
                return handler.HandleAsync(eventData);
            }
            catch (Exception ex)
            {
                string msg = $"Exception thrown when executing handler '{handler.GetType()}'{0}' for event '{eventType.Name}'{0}': {ex.Message}";
                Logger.LogError(ex, msg);
            }
            return Task.FromResult(0);
        }

        #endregion
    }
}