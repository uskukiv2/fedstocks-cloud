namespace fed.cloud.shopping.domain.Entities;

public class ShoppingListLine
{
    public Guid Id { get; set; }

    public bool IsChecked { get; set; }

    public string ProductBrand { get; set; }

    public string ProductName { get; set; }

    public long ProductNumber { get; set; }

    public decimal UnitPrice { get; set; }

    public double Quantity { get; set; }

    public Unit Unit { get; set; }

    public Seller Seller { get; set; }
}