using fed.cloud.eventbus.Base;
using System;
using System.Collections.Generic;

namespace fed.cloud.eventbus.EventBus.Abstraction
{
    public interface IEventBusSubscribeManager
    {
        bool IsEmpty { get; }

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        void RemoveSubscribtion<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        bool HasEventHandling<T>() where T : IntegrationEvent;

        void Clear();

        IEnumerable<SubscribtionInfo> GetHandlers<T>() where T : IntegrationEvent;
        IEnumerable<SubscribtionInfo> GetHandlers(string eventName);
        string GetEventName<T>() where T : IntegrationEvent;
        bool AnySubscriptionForEvent(string eventName);
        bool HasEventHandling(string eventName);
        Type GetEventTypeByName(string eventName);

        event EventHandler<string> OnEventRemoved;
    }
}
