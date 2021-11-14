using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.EventBus.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fed.cloud.eventbus.EventBus
{
    class InMemoryEventBusSubscribtionManager : IEventBusSubscribeManager
    {
        private readonly ConcurrentDictionary<string, List<SubscribtionInfo>> _handlers;
        private readonly List<Type> _knownEventTypes;

        public InMemoryEventBusSubscribtionManager()
        {
            _handlers = new ConcurrentDictionary<string, List<SubscribtionInfo>>();
            _knownEventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();

        public event EventHandler<string> OnEventRemoved;

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventName<T>();

            InternalAddSubscribtion(typeof(TH), eventName);

            if (!_knownEventTypes.Contains(typeof(T)))
            {
                _knownEventTypes.Add(typeof(T));
            }
        }

        public bool AnySubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public void Clear() => _handlers.Clear();

        public string GetEventName<T>() where T : IntegrationEvent
        {
            return typeof(T).Name;
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _knownEventTypes.FirstOrDefault(x => x.Name == eventName);
        }

        public IEnumerable<SubscribtionInfo> GetHandlers<T>() where T : IntegrationEvent
        {
            var eventName = GetEventName<T>();
            if (!_handlers.TryGetValue(eventName, out var subscribtionInfos))
            {
                return new List<SubscribtionInfo>();
            }

            return subscribtionInfos;
        }

        public IEnumerable<SubscribtionInfo> GetHandlers(string eventName)
        {
            if (!_handlers.TryGetValue(eventName, out var subscriptionInfos))
            {
                return new List<SubscribtionInfo>();
            }

            return subscriptionInfos;
        }

        public bool HasEventHandling<T>() where T : IntegrationEvent
        {
            var eventName = GetEventName<T>();
            return _handlers.ContainsKey(eventName);
        }

        public bool HasEventHandling(string eventName) => _handlers.ContainsKey(eventName);

        public void RemoveSubscribtion<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var handlerToRemove = FindSubscription<T, TH>();
            var eventName = GetEventName<T>();

            InternalRemoveSubscribtion(eventName, handlerToRemove);
        }

        private void InternalRemoveSubscribtion(string eventName, SubscribtionInfo subscribtion)
        {
            if (subscribtion == null)
            {
                return;

            }

            if(!_handlers.TryGetValue(eventName, out var subscribtions))
            {
                return;
            }

            subscribtions.Remove(subscribtion);

            if (!_handlers[eventName].Any())
            {
                if (!_handlers.TryRemove(eventName, out _))
                {
                    return;
                }

                var eventType = _knownEventTypes.FirstOrDefault(e => e.Name == eventName);
                if (eventType != null)
                {
                    _knownEventTypes.Remove(eventType);
                }
                OnEventRemoved?.Invoke(this, eventName);
            }
        }

        private SubscribtionInfo FindSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventName<T>();
            return InternalFindSubscribtion(eventName, typeof(TH));
        }

        private SubscribtionInfo InternalFindSubscribtion(string eventName, Type handlerType)
        {
            if (!HasEventHandling(eventName))
            {
                return null;
            }

            if (!_handlers.TryGetValue(eventName, out var subscribtions))
            {
                return null;
            }

            return subscribtions.FirstOrDefault(x => x.HandlerType == handlerType);
        }

        private void InternalAddSubscribtion(Type handlerType, string eventName)
        {
            if (!HasEventHandling(eventName))
            {
                _handlers.TryAdd(eventName, new List<SubscribtionInfo>());
            }

            if (_handlers.TryGetValue(eventName, out var infos) && infos.Any(x => x.HandlerType == handlerType))
            {
                throw new DuplicateWaitObjectException($"Handler type {handlerType.Name} already registered for {eventName}");
            }

            infos.Add(SubscribtionInfo.Create(handlerType));
        }
    }
}
