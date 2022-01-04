using fedstocks.cloud.web.api.Models;

namespace fedstocks.cloud.web.api.Services;

public interface IShoppingService
{
    Task<CompletedShoppingList> CreateShoppingListAsync(NewShoppingList newList, Guid userId);
    Task<CompletedShoppingList> UpdateShoppingListAsync(CompletedShoppingList list, Guid userId);
    Task<bool> DeleteShoppingListAsync(Guid userId, int shoppingListId);
    Task<ShoppingCheckoutResult> CheckoutShoppingListAsync(Guid userId, int shoppingListId);
    Task<IEnumerable<CompletedShoppingList>> GetShoppingListsAsync(Guid userId);
    Task<CompletedShoppingList> GetShoppingListAsync(Guid userId, int shoppingList);
}