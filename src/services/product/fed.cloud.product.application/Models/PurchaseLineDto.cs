using System;

namespace fed.cloud.product.application.Models;

public class PurchaseLineDto
{
    public long Number { get; set; }

    public string Brand { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public decimal OriginalPrice { get; set; }

    public Guid Seller { get; set; }

    public int UnitId { get; set; }

    public int CategoryId { get; set; }
}