using fed.cloud.store.application.Extensions;
using fed.cloud.store.application.Models;
using fed.cloud.store.domain.Root.Order;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Commands;

public class UpdateOrderStatusToIntermediateStatusCommand : IRequest<OrderSummary>
{
    public UpdateOrderStatusToIntermediateStatusCommand(Guid orderOwner, string orderNumber, int orderStatusId)
    {
        OrderNumber = orderNumber;
        OrderStatusId = orderStatusId;
        OrderOwner = orderOwner;
    }

    public Guid OrderOwner { get; }

    public string OrderNumber { get; }

    public int OrderStatusId { get; }
}

public class
    UpdateOrderStatusToIntermediateStatusCommandHandler : IRequestHandler<UpdateOrderStatusToIntermediateStatusCommand,
        OrderSummary>
{
    private readonly ILogger<UpdateOrderStatusToIntermediateStatusCommandHandler> _logger;
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusToIntermediateStatusCommandHandler(IOrderRepository orderRepository,
        ILogger<UpdateOrderStatusToIntermediateStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<OrderSummary> Handle(UpdateOrderStatusToIntermediateStatusCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var orderStatus = await _orderRepository.GetStatusAsync(request.OrderOwner, request.OrderStatusId);
            var currentOrder = await _orderRepository.GetAsync(request.OrderNumber);

            if (currentOrder.OrderOwner != request.OrderOwner)
                return OrderSummary.CreateIssuedOrder("order isn't exists");

            currentOrder.StatusId = orderStatus.Id;

            _orderRepository.Update(currentOrder, cancellationToken);

            return currentOrder.CreateOrderSummaryFromOrder(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot update order {orderId} status to {statusId} for owner {ownerId}",
                request.OrderNumber, request.OrderStatusId, request.OrderOwner);
            return new OrderSummary();
        }
    }
}