using fed.cloud.store.application.Models;
using fed.cloud.store.domain.Root.Order;
using fed.cloud.store.domain.Root.Stock;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Commands;

public class ProcessCompletedOrderCommand : IRequest
{
    public ProcessCompletedOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}

public class ProcessCompletedOrderCommandHandler : IRequestHandler<ProcessCompletedOrderCommand>
{
    private readonly ILogger<ProcessCompletedOrderCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IOrderRepository _orderRepository;
    private readonly IStockRepository _stockRepository;

    public ProcessCompletedOrderCommandHandler(IMediator mediator, IOrderRepository orderRepository,
        IStockRepository stockRepository, ILogger<ProcessCompletedOrderCommandHandler> logger)
    {
        _mediator = mediator;
        _orderRepository = orderRepository;
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(ProcessCompletedOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetAsync(request.OrderId);
        if (order is null)
        {
            _logger.LogWarning($"order cannot be found with {request.OrderId}");
            return new Unit();
        }

        if (order.Status.IsCompleteState)
        {
            _logger.LogWarning($"order {request.OrderId} is not complete");
            return new Unit();
        }

        var stock = await _stockRepository.GetDefaultStockAsync(order.OrderOwner);

        var stockUpdateCommand = new NewStockTransactionCommand(stock.Id, GetTransactionItemsFromOrderItems(order));
        await _mediator.Send(stockUpdateCommand, cancellationToken).ConfigureAwait(false);

        return Unit.Value;
    }

    private static IEnumerable<NewStockTransactionCommandItem> GetTransactionItemsFromOrderItems(Order order)
    {
        return order.Items.Select(orderItem => new NewStockTransactionCommandItem(orderItem.ProductNumber,
            orderItem.ItemName, orderItem.Unit,
            TransactionOperationType.Update, 0, (int)orderItem.UnitType));
    }
}