using System;
using System.Collections.Generic;

namespace fed.cloud.store.application.Models;

public record OrderData
{
    public string OrderNumber { get; set; }

    public string SalePoint { get; set; }

    public int ReceiptId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public decimal Total { get; set; }

    public int ItemsTotal { get; set; }

    public int StatusId { get; set; }

    public IEnumerable<OrderLineData> Lines { get; set; }
}