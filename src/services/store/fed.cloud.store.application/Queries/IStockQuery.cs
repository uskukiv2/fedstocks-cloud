using fed.cloud.store.domain.Root.Stock;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Queries;

public interface IStockQuery
{
    Task<Stock> GetStockByUserIdAsync(Guid id);
    Task<IEnumerable<StockItem>> GetStockItemsAsync(string stockName);
    Task<Stock> GetGroupStockByGroupIdAsync(Guid groupId);
}