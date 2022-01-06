using fed.cloud.eventbus.Base;
using Newtonsoft.Json;
using System;
using LiteDB;

namespace fed.cloud.eventbus.Data
{
    public class IntegrationEventLogEntry
    {
        public IntegrationEventLogEntry()
        {
        }

        public IntegrationEventLogEntry(IntegrationEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            EventName = @event.GetType().Name;
            CreationTime = @event.CreationDate;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateType.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }

        private IntegrationEventLogEntry(string content,
            EventStateType state,
            DateTime creationTime,
            Guid eventId,
            string eventName,
            IntegrationEvent integrationEvent,
            int timesSent,
            string transactionId)
        {
            Content = content;
            State = state;
            CreationTime = creationTime;
            EventId = eventId;
            EventName = eventName;
            IntegrationEvent = integrationEvent;
            TimesSent = timesSent;
            TransactionId = transactionId;
        }

        [BsonId]
        public Guid EventId { get; }

        public string EventName { get; }

        public IntegrationEvent IntegrationEvent { get; internal set; }
        
        public EventStateType State { get; internal set; } 
        
        public int TimesSent { get; internal set; }
        
        public DateTime CreationTime { get; }
        
        public string Content { get; }
        
        public string TransactionId { get; }

        public static IntegrationEventLogEntry Restore(string content,
            EventStateType state,
            DateTime creationTime,
            Guid eventId,
            string eventName,
            IntegrationEvent integrationEvent,
            int timesSent,
            string transactionId)
        {
            return new IntegrationEventLogEntry(content,
                state,
                creationTime,
                eventId,
                eventName,
                integrationEvent,
                timesSent,
                transactionId);
        }
    }
}
