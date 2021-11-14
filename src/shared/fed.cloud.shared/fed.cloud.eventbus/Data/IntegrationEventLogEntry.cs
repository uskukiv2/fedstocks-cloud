using fed.cloud.eventbus.Base;
using Newtonsoft.Json;
using System;

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

        public Guid EventId { get; }

        public string EventName { get; }

        public IntegrationEvent IntegrationEvent { get; internal set; }
        
        public EventStateType State { get; internal set; } 
        
        public int TimesSent { get; internal set; }
        
        public DateTime CreationTime { get; }
        
        public string Content { get; }
        
        public string TransactionId { get; }
    }
}
