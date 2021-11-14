using System;
using fed.cloud.eventbus.Base;

namespace fed.cloud.store.application.IntegrationEvents.Events;

/// <summary>
///     Current exist item should be removed
/// </summary>
/// ///
/// <remarks>if item doesn't exist - skip</remarks>
public record StockTransactionRemoveItemEvent : IntegrationEvent
{
    public StockTransactionRemoveItemEvent(long productNumber, Guid stockId)
    {
        ProductNumber = productNumber;
        StockId = stockId;
    }

    public long ProductNumber { get; }

    public Guid StockId { get; }
}

/// <summary>
///     Current exist item should be updated
/// </summary>
public record StockTransactionUpdateItemEvent : IntegrationEvent
{
    public StockTransactionUpdateItemEvent(long productNumber, double quantity, Guid stockId)
    {
        ProductNumber = productNumber;
        Quantity = quantity;
        StockId = stockId;
    }

    public long ProductNumber { get; }

    public double Quantity { get; }

    public Guid StockId { get; }
}

/// <summary>
///     New item should be created and add
/// </summary>
/// <remarks>if item already exist - skip</remarks>
public record StockTransactionAddItemEvent : IntegrationEvent
{
    public StockTransactionAddItemEvent(long productNumber,
        string productName,
        double quantity,
        int categoryId,
        int unitId,
        Guid stockId)
    {
        ProductNumber = productNumber;
        ProductName = productName;
        Quantity = quantity;
        CategoryId = categoryId;
        UnitId = unitId;
        StockId = stockId;
    }

    public long ProductNumber { get; }

    public string ProductName { get; }

    public double Quantity { get; }

    public int CategoryId { get; }

    public int UnitId { get; }

    public Guid StockId { get; }
}