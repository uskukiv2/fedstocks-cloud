using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.Data;
using fed.cloud.eventbus.Extensions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace fed.cloud.eventbus
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly string _connectionString;
        private readonly List<Type> _eventTypes;

        public IntegrationEventLogService(IEventServiceConfiguration configuration)
        {
            _connectionString = configuration.EventDatabase.Connection;
            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly()?.FullName)
                .GetTypes()
                .Where(t => t.BaseType == typeof(IntegrationEvent))
                .ToList();
        }

        public IEnumerable<IntegrationEventLogEntry> GetPendingEventLogs(Guid transactionId)
        {
            using var db = new LiteRepository(_connectionString);
            var events = db.Database.GetCollection<IntegrationEventLogEntry>().Query()
                .Where(x => x.TransactionId == transactionId.ToString())
                .Where(x => x.State == EventStateType.NotPublished)
                .Select(x => new { x.EventId, x.EventName, x.CreationTime,x.Content,x.State, x.TimesSent, x.TransactionId, x.IntegrationEvent }).ToList()
                .Select(x => IntegrationEventLogEntry.Restore(x.Content,
                    x.State,
                    x.CreationTime,
                    x.EventId,
                    x.EventName,
                    x.IntegrationEvent,
                    x.TimesSent,
                    x.TransactionId));

            if (events != null && events.Any())
            {
                return events.OrderBy(x => x.CreationTime)
                    .Select(e => e
                        .Deserilize(_eventTypes.Find(x => x.Name == e.EventName)));
            }

            return new List<IntegrationEventLogEntry>();
        }

        public void MarkEventAsFailed(Guid eventId)
        {
            InternalMarkEventNewStatus(eventId, EventStateType.Failed);
        }

        public void MarkEventAsInProgress(Guid eventId)
        {
            InternalMarkEventNewStatus(eventId, EventStateType.InProgress);
        }

        public void MarkEventAsPublished(Guid eventId)
        {
            InternalMarkEventNewStatus(eventId, EventStateType.Published);
        }

        public void SaveEventToTransaction<T>(T @event, Guid transaction) where T : IntegrationEvent
        {
            if (transaction == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(transaction));
            }

            using var db = new LiteRepository(_connectionString);
            if (!db.Database.BeginTrans())
            {
                return;
            }

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction);

            db.Insert(eventLogEntry);
            db.Database.Commit();
            db.Database.Checkpoint();
        }

        private void InternalMarkEventNewStatus(Guid evenId, EventStateType eventState)
        {
            using var db = new LiteRepository(_connectionString);
            var selectEventLogEntry = db.Query<IntegrationEventLogEntry>()
                .Where(x => x.EventId == evenId)
                .Select(x => new
                {
                    x.EventId, x.EventName, x.CreationTime, x.Content, x.State, x.TimesSent, x.TransactionId,
                    x.IntegrationEvent
                })
                .ForUpdate()
                .Single();
            var eventLogEntry = IntegrationEventLogEntry.Restore(
                selectEventLogEntry.Content,
                selectEventLogEntry.State,
                selectEventLogEntry.CreationTime,
                selectEventLogEntry.EventId,
                selectEventLogEntry.EventName, 
                selectEventLogEntry.IntegrationEvent,
                selectEventLogEntry.TimesSent, 
                selectEventLogEntry.TransactionId);
            eventLogEntry.State = eventState;
            db.Update(eventLogEntry);
            db.Database.Checkpoint();
        }
    }
}