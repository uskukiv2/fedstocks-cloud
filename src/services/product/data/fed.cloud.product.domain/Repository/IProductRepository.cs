using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.domain.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<int> BulkInsertAsync(IEnumerable<Product> products, CancellationToken token);

        Task<IEnumerable<Product>> TTSearchAsync(string query, CancellationToken token);
        Task<Product> GetByNumberAsync(long number);
        Task AddPurchaseForProductAsync(Guid id, decimal price, decimal originalPrice, Guid sellerId);
    }
}
