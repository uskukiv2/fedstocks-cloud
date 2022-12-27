using fed.cloud.store.domain.Abstract;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.store.domain.Root.Stock
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<Stock> GetDefaultStockAsync(Guid userId);
        Task<IEnumerable<Stock>> GetStocksAsync(Guid userId);
        Task<bool> IsStockExist(Guid stockId);
        Task<Stock> GetGroupStockAsync(Guid groupId);

        Task<int> RemoveStockItems(Guid stockId, IEnumerable<StockItem> stockItemsToRemove, CancellationToken token = default);
    }
}