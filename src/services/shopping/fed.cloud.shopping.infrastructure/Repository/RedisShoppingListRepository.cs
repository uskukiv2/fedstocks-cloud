using fed.cloud.shopping.domain.Entities;
using fed.cloud.shopping.domain.Repositories;
using fed.cloud.shopping.infrastructure.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace fed.cloud.shopping.infrastructure.Repository;

public class RedisShoppingListRepository : IShoppingListRepository
{
    private readonly ILogger<RedisShoppingListRepository> _logger;
    private readonly ICacheClient _cacheClient;

    public RedisShoppingListRepository(ILogger<RedisShoppingListRepository> logger, ICacheClient cacheClient)
    {
        _logger = logger;
        _cacheClient = cacheClient;
    }

    public async Task<IEnumerable<ShoppingList>> GetUserShoppingListsAsync(Guid userId)
    {
        var entries = await _cacheClient.Database.HashGetAllAsync(new RedisKey($"shp:{userId}"));
        if (entries != null && entries.Length != 0)
        {
            return entries.Select(x => ParseEntry(x.Value))!;
        }

        _logger.LogWarning("no lists for user {0}", userId);
        return null!;
    }

    public async Task<ShoppingList> GetShoppingListAsync(Guid userId, int shoppingId)
    {
        var result = await _cacheClient.Database.HashGetAsync(new RedisKey($"shp:{userId}"), new RedisValue(shoppingId.ToString()));
        if (!result.IsNullOrEmpty)
        {
            return JsonConvert.DeserializeObject<ShoppingList>(result)!;
        }

        _logger.LogWarning("is not able to find shopping list {0} for user {1}", shoppingId, userId);
        return null!;
    }

    public async Task<ShoppingList?> FullUpdateShoppingListAsync(ShoppingList list)
    {
        var hashEntry = new HashEntry(list.Id.ToString(), JsonConvert.SerializeObject(list));
        await _cacheClient.Database.HashSetAsync(new RedisKey($"shp:{list.UserId}"), new[] { hashEntry });

        return await GetShoppingListAsync(list.UserId, list.Id);
    }

    public async Task<bool> DestroyListAsync(Guid userId, int listId)
    {
        return await _cacheClient.Database.HashDeleteAsync(new RedisKey($"shp:{userId}"), new RedisValue(listId.ToString()));
    }

    public async Task<ShoppingList?> CreateShoppingListAsync(ShoppingList list)
    {
        var total = await _cacheClient.Database.HashGetAllAsync(new RedisKey($"shp:{list.UserId}"));
        list.Id = total.Length + 1;

        return await FullUpdateShoppingListAsync(list);
    }

    private static ShoppingList? ParseEntry(RedisValue arg)
    {
        return JsonConvert.DeserializeObject<ShoppingList>(arg);
    }
}