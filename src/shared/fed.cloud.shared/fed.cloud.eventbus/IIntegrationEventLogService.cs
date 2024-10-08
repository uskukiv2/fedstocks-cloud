﻿using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.Data;
using System;
using System.Collections.Generic;

namespace fed.cloud.eventbus
{
    public interface IIntegrationEventLogService
    {
        void SaveEventToTransaction<T>(T @event, Guid transaction) where T : IntegrationEvent;
        IEnumerable<IntegrationEventLogEntry> GetPendingEventLogs(Guid transactionId);
        void MarkEventAsInProgress(Guid eventId);
        void MarkEventAsPublished(Guid eventId);
        void MarkEventAsFailed(Guid eventId);
    }
}
