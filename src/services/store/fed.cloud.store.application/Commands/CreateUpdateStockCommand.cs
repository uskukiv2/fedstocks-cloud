using fed.cloud.eventbus.Base;
using fed.cloud.store.application.IntegrationEvents;
using fed.cloud.store.application.IntegrationEvents.Events;
using fed.cloud.store.application.Models;
using fed.cloud.store.domain.Root.Stock;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Commands;

public class NewStockTransactionCommandItem
{
    public NewStockTransactionCommandItem(
        long productNumber,
        string productName,
        double itemQuantity,
        TransactionOperationType itemOperationType,
        int itemCategoryId,
        int itemUnitId)
    {
        ProductNumber = productNumber;
        ProductName = productName;
        Quantity = itemQuantity;
        OperationType = itemOperationType;
        CategoryId = itemCategoryId;
        UnitId = itemUnitId;
    }


    public long ProductNumber { get; }

    public string ProductName { get; }

    public double Quantity { get; }

    public TransactionOperationType OperationType { get; }

    public int CategoryId { get; }

    public int UnitId { get; }
}

public class NewStockTransactionCommand : IRequest<Guid>
{
    public NewStockTransactionCommand(Guid stockId, IEnumerable<NewStockTransactionCommandItem> items)
    {
        StockId = stockId;
        Items = items;
        TransactionId = Guid.NewGuid();
    }

    public Guid StockId { get; }

    public IEnumerable<NewStockTransactionCommandItem> Items { get; }

    public Guid TransactionId { get; }
}

public class NewStockTransactionCommandHandler : IRequestHandler<NewStockTransactionCommand, Guid>
{
    private readonly IStoreIntegrationEventService _eventService;
    private readonly ILogger<NewStockTransactionCommandHandler> _logger;
    private readonly IStockRepository _stockRepository;

    public NewStockTransactionCommandHandler(IStockRepository stockRepository,
        IStoreIntegrationEventService eventService,
        ILogger<NewStockTransactionCommandHandler> logger)
    {
        _stockRepository = stockRepository;
        _eventService = eventService;
        _logger = logger;
    }

    public async Task<Guid> Handle(NewStockTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = await _stockRepository.IsStockExist(request.StockId);
        if (!result)
        {
            _logger.LogWarning($"Can't find the stock with id {request.StockId}");
            return Guid.Empty;
        }

        var transactionId = await _eventService.BeginLocalTransaction();
        foreach (var item in request.Items)
        {
            IntegrationEvent @event = null;
            switch (item.OperationType)
            {
                case TransactionOperationType.Update:
                    @event = new StockTransactionUpdateItemEvent(item.ProductNumber, item.Quantity, request.StockId);
                    break;
                case TransactionOperationType.New:
                    @event = new StockTransactionAddItemEvent(item.ProductNumber, item.ProductName, item.Quantity,
                        item.CategoryId,
                        item.UnitId, request.StockId);
                    break;
                case TransactionOperationType.Remove:
                    @event = new StockTransactionRemoveItemEvent(item.ProductNumber, request.StockId);
                    break;
            }

            await _eventService.CommitEventAsync(@event, transactionId);
        }

        _logger.LogInformation("Push transaction {transactionId}", transactionId);
        await _eventService.CompleteTransaction(transactionId).ConfigureAwait(false);

        return request.TransactionId;
    }
}