using fed.cloud.store.application.Models;
using fed.cloud.store.domain.Root.Order;
using System.Linq;

namespace fed.cloud.store.application.Extensions;

public static class OrderSummaryExtensions
{
    public static OrderSummary CreateOrderSummaryFromOrder(this Order order, bool isSuccess)
    {
        return new OrderSummary
        {
            orderId = order.OrderNumber,
            date = order.StartedAt,
            itemsTotal = isSuccess ? order.Items.Count() : 0,
            status = $"[{order.Status.StatusId}] // {order.Status.StatusName}",
            total = order.Items.Sum(x => x.ActualPrice)
        };
    }
}