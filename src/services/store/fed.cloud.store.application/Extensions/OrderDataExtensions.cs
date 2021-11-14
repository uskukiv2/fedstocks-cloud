using System.Linq;
using fed.cloud.store.application.Models;
using fed.cloud.store.domain.Root.Order;

namespace fed.cloud.store.application.Extensions;

public static class OrderDataExtensions
{
    public static OrderData CreateOrderDataFromOrder(this Order order)
    {
        return new OrderData
        {
            OrderNumber = order.GetOrderNumber(),
            CreatedAt = order.GetOrderDate(),
            CreatedBy = order.GetOrderOwner(),
            ItemsTotal = order.Items.Count(),
            ReceiptId = order.GetReceiptNumber(),
            SalePoint = string.Empty,
            Total = order.Items.Sum(x => x.GetPrice()),
            Status = order.GetOrderStatus().ToString(),
            Lines = order.Items.Select(ToOrderLineData)
        };
    }

    public static OrderLineData ToOrderLineData(this OrderItem item)
    {
        return new OrderLineData
        {
            Name = item.GetName(),
            UnitString = item.GetUnitType().ToString(),
            TotalPerUnit = item.GetPrice(),
            TotalPerStandart = decimal.MinValue
        };
    }
}