using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fed.cloud.store.domain.Abstract;

namespace fed.cloud.store.domain.Root.Stock
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<IEnumerable<Stock>> GetStocksAsync(Guid userId);
        Task<bool> IsStockExist(Guid stockId);
        Task<Stock> GetGroupStockAsync(Guid groupId);
    }
}