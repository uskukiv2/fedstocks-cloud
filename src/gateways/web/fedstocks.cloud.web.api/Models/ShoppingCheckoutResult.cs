namespace fedstocks.cloud.web.api.Models;

public class ShoppingCheckoutResult
{
    public ShoppingCheckoutResult()
    {
        ShoppingId = -1;
    }

    public int ShoppingId { get; set; }

    public int TotalLines { get; set; }

    public string Name { get; set; }

    public bool IsSuccess { get; set; }
}