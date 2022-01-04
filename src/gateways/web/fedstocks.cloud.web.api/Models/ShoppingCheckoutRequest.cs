using System.ComponentModel.DataAnnotations;

namespace fedstocks.cloud.web.api.Models;

public class ShoppingCheckoutRequest
{
    [Required]
    public int ShoppingListId { get; set; }

    public bool IsForceCheckout { get; set; }
}