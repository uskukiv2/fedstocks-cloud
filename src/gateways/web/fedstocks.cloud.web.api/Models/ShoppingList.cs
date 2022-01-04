namespace fedstocks.cloud.web.api.Models;

public abstract class BaseShoppingList
{
    public string Name { get; set; }

    public IEnumerable<ShoppingListLine> Lines { get; set; }

}

public class NewShoppingList : BaseShoppingList
{
}

public class CompletedShoppingList : BaseShoppingList
{
    public CompletedShoppingList()
    {
        Id = -1;
    }

    public int Id { get; set; }
}

public class ShoppingListLine
{
    public bool IsChecked { get; set; }

    public string ProductBrand { get; set; }

    public string ProductName { get; set; }

    public long ProductNumber { get; set; }

    public decimal UnitPrice { get; set; }

    public double Quantity { get; set; }

    public Unit Unit { get; set; }

    public Seller Seller { get; set; }
}

public class Unit
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Rate { get; set; }
}

public class Seller
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid CountyId { get; set; }
}