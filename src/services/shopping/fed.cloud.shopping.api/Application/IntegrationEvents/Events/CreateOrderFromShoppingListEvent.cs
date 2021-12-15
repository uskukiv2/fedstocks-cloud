using fed.cloud.eventbus.Base;
using fed.cloud.shopping.api.Protos;

namespace fed.cloud.shopping.api.Application.IntegrationEvents.Events;

public record CreateOrderFromShoppingListEvent(Guid UserId, DateTime CompletedAt, OrderLine[] ShoppingLines) : IntegrationEvent;

public class OrderLine
{
    public string Name { get; set; }

    public long Number { get; set; }

    public double Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public Unit Unit { get; set; }
}