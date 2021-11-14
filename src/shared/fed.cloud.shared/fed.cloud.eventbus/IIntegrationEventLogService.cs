using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace fed.cloud.eventbus
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventToTransactionAsync(IntegrationEvent @event, Guid transaction);
        Task<IEnumerable<IntegrationEventLogEntry>> GetPendingEventLogsAsync(Guid transactionId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
