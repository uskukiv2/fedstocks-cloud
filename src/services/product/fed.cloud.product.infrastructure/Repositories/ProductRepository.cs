using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;

namespace fed.cloud.product.infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public ProductRepository(ProductContext)
        {
                
        }

        public IUnitOfWork UnitOfWork { get; }

        public Task<Product> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(Product entity, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Update(Product entity, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> BulkInsertAsync(IEnumerable<Product> products, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> TTSearchAsync(string query, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByNumberAsync(long number)
        {
            throw new NotImplementedException();
        }

        public Task AddPurchaseForProductAsync(Guid id, decimal price, decimal originalPrice, Guid sellerId)
        {
            throw new NotImplementedException();
        }
    }
}