using fed.cloud.shopping.domain.Entities;

namespace fed.cloud.shopping.domain.Repositories;

public interface IShoppingListRepository
{
    Task<IEnumerable<ShoppingList>> GetUserShoppingListsAsync(Guid userId);
    Task<ShoppingList> GetShoppingListAsync(Guid id, int shoppingId);
    Task<ShoppingList?> FullUpdateShoppingListAsync(ShoppingList list);
    Task<bool> DestroyListAsync(Guid userId, int listId);
}