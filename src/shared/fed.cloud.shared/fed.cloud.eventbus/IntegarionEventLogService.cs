using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.Data;
using fed.cloud.eventbus.Extensions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace fed.cloud.eventbus
{
    public class IntegarionEventLogService : IIntegrationEventLogService
    {
        private readonly string _connectionString;
        private readonly List<Type> _eventTypes;

        public IntegarionEventLogService(IEventServiceConfiguration configuration)
        {
            _connectionString = configuration.EventDatabase.Connection;
            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> GetPendingEventLogsAsync(Guid transactionId)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var db = new LiteRepository(_connectionString))
                {
                    var events = db.Query<IntegrationEventLogEntry>()
                    .Where(x => x.TransactionId == transactionId.ToString())
                    .Where(x => x.State == EventStateType.NotPublished)
                    .ToList();

                    if (events != null && events.Any()) 
                    {
                        return events.OrderBy(x => x.CreationTime)
                         .Select(e => e
                         .Deserilize(_eventTypes.Find(x => x.Name == e.EventName)));
                    }

                    return new List<IntegrationEventLogEntry>();
                }
            });
        }

        public async Task MarkEventAsFailedAsync(Guid eventId)
        {
            await InternalMarkEventNewStatus(eventId, EventStateType.Failed);
        }

        public async Task MarkEventAsInProgressAsync(Guid eventId)
        {
            await InternalMarkEventNewStatus(eventId, EventStateType.InProgress);
        }

        public async Task MarkEventAsPublishedAsync(Guid eventId)
        {
            await InternalMarkEventNewStatus(eventId, EventStateType.Published);
        }

        public Task SaveEventToTransactionAsync(IntegrationEvent @event, Guid transaction)
        {
            if (transaction == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(transaction));
            }

            return Task.Factory.StartNew(() => 
            {
                using (var db = new LiteRepository(_connectionString))
                {
                    var eventLogEntry = new IntegrationEventLogEntry(@event, transaction);

                    db.Insert(eventLogEntry);
                }
            });
        }

        private Task InternalMarkEventNewStatus(Guid evenId, EventStateType eventState)
        {
            return Task.Factory.StartNew(() => 
            {
                using (var db = new LiteRepository(_connectionString))
                {
                    var eventLogEntry = db.Query<IntegrationEventLogEntry>().Where(x => x.EventId == evenId).ForUpdate().Single();
                    eventLogEntry.State = eventState;
                    db.Update(eventLogEntry);
                }
            });
        }
    }
}
