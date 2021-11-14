﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fed.cloud.eventbus;
using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.EventBus.Abstraction;
using Microsoft.Extensions.Logging;

namespace fed.cloud.store.application.IntegrationEvents;

public interface IStoreIntegrationEventService
{
    Task<Guid> BeginLocalTransaction();
    Task CompleteTransaction(Guid transactionId);
    Task CommitEventAsync(IntegrationEvent @event, Guid transactionId);
}

public class StoreIntegrationEventService : IStoreIntegrationEventService
{
    private readonly IEventBus _eventBus;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<StoreIntegrationEventService> _logger;
    private readonly IList<TransactionInfo> _transactions;

    public StoreIntegrationEventService(IEventBus eventBus, IIntegrationEventLogService integrationEventLogService,
        ILogger<StoreIntegrationEventService> logger)
    {
        _eventBus = eventBus;
        _eventLogService = integrationEventLogService;
        _logger = logger;
        _transactions = new List<TransactionInfo>();
    }

    public Task<Guid> BeginLocalTransaction()
    {
        var newTransaction = new TransactionInfo();
        _transactions.Add(newTransaction);

        return Task.FromResult(newTransaction.TransactionId);
    }

    public async Task CompleteTransaction(Guid transactionId)
    {
        if (_transactions.All(x => x.TransactionId != transactionId))
            await Task.FromException(new Exception($"Transaction with {transactionId} is not exist"));
        var pendingEventLogs = (await _eventLogService.GetPendingEventLogsAsync(transactionId)).ToList();
        foreach (var @event in pendingEventLogs)
        {
            _logger.LogInformation($"Prepare event: {@event.EventId} - {@event.EventName}  {@event.IntegrationEvent}");

            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(@event.EventId);
                _eventBus.Publish(@event.IntegrationEvent);
                await _eventLogService.MarkEventAsPublishedAsync(@event.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"===== caught an error while try to publish event {@event.EventId} ===== \n {ex.Message}");

                await _eventLogService.MarkEventAsFailedAsync(@event.EventId);
            }
        }

        try
        {
            var transactionToUpdate = _transactions.FirstOrDefault(x => x.TransactionId == transactionId);
            pendingEventLogs = (await _eventLogService.GetPendingEventLogsAsync(transactionId)).ToList();
            if (!pendingEventLogs.Any())
            {
                _transactions.Remove(transactionToUpdate);
                return;
            }

            transactionToUpdate?.UpdateEventsCount(pendingEventLogs.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task CommitEventAsync(IntegrationEvent @event, Guid transactionId)
    {
        if (_transactions.All(x => x.TransactionId != transactionId))
            await Task.FromException(new Exception($"Transaction with {transactionId} is not exist"));

        _logger.LogInformation($"Commit event {@event.Id} with {@event}");

        await _eventLogService.SaveEventToTransactionAsync(@event, transactionId);
        _transactions.FirstOrDefault(x => x.TransactionId == transactionId)?.UpdateEventsCount(x => x++);
    }
}