using fed.cloud.store.application.Commands;
using fed.cloud.store.application.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace fed.cloud.store.api.Services;

public class OrderService : Order.OrderBase
{
    private readonly IMediator _mediator;
    private readonly IOrderQuery _orderQuery;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IMediator mediator, IOrderQuery orderQuery, ILogger<OrderService> logger)
    {
        _mediator = mediator;
        _orderQuery = orderQuery;
        _logger = logger;
    }

    public override async Task<OrderSummary> CompleteOrder(OrderRequest request, ServerCallContext context)
    {
        _logger.LogTrace("Begin complete order {id}", request.OrderId);

        using (_logger.BeginScope("order"))
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var command = new UpdateOrderStatusToCompleteCommand(userId, request.OrderId);
                var summary = await _mediator.Send(command, context.CancellationToken);

                context.Status = Status.DefaultSuccess;
                return MapToOrderSummary(summary);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "processing order {id} stopped", request.OrderId);
                context.Status = new Status(StatusCode.DeadlineExceeded, e.Message, e);
                return new OrderSummary();
            }
        }
    }

    // for third version of fedstocks
    public override Task<OrderSummary> ConfirmOrder(OrderDraftData request, ServerCallContext context)
    {
        return base.ConfirmOrder(request, context);
    }

    // for third version of fedstocks
    public override Task<OrderData> CreateOrder(ShoppingItems request, ServerCallContext context)
    {
        return base.CreateOrder(request, context);
    }

    public override async Task<OrderData> GetOrder(OrderRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Begin get order for {id}", request.UserId);
        if (!Guid.TryParse(request.UserId, out var guid) || guid == Guid.Empty)
        {
            context.Status = new Status(StatusCode.Aborted, "cannot get user id");

            return new OrderData();
        }

        try
        {
            var order = await _orderQuery.GetOrderAsync(request.OrderId);
            if (order == null || order.OrderOwner != guid)
            {
                context.Status = new Status(StatusCode.DataLoss, $"cannot get order {request.OrderId} for user {guid}");
                return new OrderData();
            }

            context.Status = Status.DefaultSuccess;
            return MapToOrderData(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot get order for {id}", guid);
            context.Status = new Status(StatusCode.DeadlineExceeded, $"{ex.Message}", ex);
            return new OrderData();
        }
    }

    public override async Task<OrdersResponse> GetOrders(OrdersRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Begin get orders for {id}", request.UserId);
        if (!Guid.TryParse(request.UserId, out var guid) || guid == Guid.Empty)
        {
            context.Status = new Status(StatusCode.Aborted, "cannot get user id");

            return new OrdersResponse();
        }

        try
        {
            var orders = (await _orderQuery.GetOrdersByOwnerIdAsync(guid)).ToList();
            if (!orders.Any())
            {
                context.Status = new Status(StatusCode.DataLoss, "cannot get orders for user");
                return new OrdersResponse();
            }

            context.Status = Status.DefaultSuccess;
            return MapToOrdersResponse(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot get orders for {id}", guid);
            context.Status = new Status(StatusCode.DeadlineExceeded, $"{ex.Message}", ex);
            return new OrdersResponse();
        }
    }

    // for third version of fedstocks
    public override Task<OrderSummary> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
    {
        return base.UpdateOrderStatus(request, context);
    }

    private static OrdersResponse MapToOrdersResponse(List<domain.Root.Order.Order> orders)
    {
        var orderResponse = new OrdersResponse();

        orders.ForEach(x => orderResponse.Orders.Add(MapToOrderSummary(x)));

        return orderResponse;
    }

    private static OrderSummary MapToOrderSummary(domain.Root.Order.Order order)
    {
        return new OrderSummary
        {
            OrderId = order.OrderNumber,
            CreatedAt = Timestamp.FromDateTime(order.StartedAt),
            Status = order.Status.StatusName,
            TotalAmount = decimal.ToDouble(order.Items.Sum(x => x.ActualPrice)),
            TotalItems = order.Items.Count
        };
    }

    private static OrderSummary MapToOrderSummary(application.Models.OrderSummary summary)
    {
        return new OrderSummary
        {
            OrderId = summary.orderId,
            CreatedAt = Timestamp.FromDateTime(summary.date),
            Status = summary.status,
            TotalAmount = decimal.ToDouble(summary.total),
            TotalItems = summary.itemsTotal
        };
    }

    private static OrderData MapToOrderData(domain.Root.Order.Order order)
    {
        return new OrderData
        {
            OrderId = order.OrderNumber,
            CreatedAt = Timestamp.FromDateTime(order.StartedAt),
            Status = order.Status.StatusName
        };
    }
}