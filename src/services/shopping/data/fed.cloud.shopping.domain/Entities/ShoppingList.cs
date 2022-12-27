namespace fed.cloud.shopping.domain.Entities;

public class ShoppingList
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; }

    public Seller Seller { get; set; }

    public IEnumerable<ShoppingListLine> Lines { get; set; }
}