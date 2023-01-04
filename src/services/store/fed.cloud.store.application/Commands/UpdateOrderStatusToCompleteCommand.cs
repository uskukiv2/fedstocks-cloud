using fed.cloud.store.application.Extensions;
using fed.cloud.store.application.Models;
using fed.cloud.store.domain.Root.Order;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Commands;

public class UpdateOrderStatusToCompleteCommand : IRequest<OrderSummary>
{
    public UpdateOrderStatusToCompleteCommand(Guid orderOwner, string orderNumber)
    {
        OrderOwner = orderOwner;
        OrderNumber = orderNumber;
    }

    public Guid OrderOwner { get; }

    public string OrderNumber { get; }
}

public class
    UpdateOrderStatusToCompleteCommandHandler : IRequestHandler<UpdateOrderStatusToCompleteCommand, OrderSummary>
{
    private readonly ILogger<UpdateOrderStatusToCompleteCommandHandler> _logger;
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusToCompleteCommandHandler(IOrderRepository orderRepository,
        ILogger<UpdateOrderStatusToCompleteCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderSummary> Handle(UpdateOrderStatusToCompleteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentOrder = await _orderRepository.GetAsync(request.OrderNumber);
            var orderStatus = await _orderRepository.GetCompleteOrderStatusAsync(request.OrderOwner);

            if (currentOrder.OrderOwner != request.OrderOwner)
                return OrderSummary.CreateIssuedOrder("order isn't exists");

            currentOrder.StatusId = orderStatus.Id;

            _orderRepository.Update(currentOrder, cancellationToken);

            return currentOrder.CreateOrderSummaryFromOrder(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Order {orderNumber} status cannot be updated to complete. User owner {owner}",
                request.OrderNumber, request.OrderOwner);
            return new OrderSummary();
        }
    }
}