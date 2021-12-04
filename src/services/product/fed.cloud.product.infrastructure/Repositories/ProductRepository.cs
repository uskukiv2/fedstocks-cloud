using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace fed.cloud.product.infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Product> GetAsync(Guid id)
        {
            return await _context.Products.AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Add(Product entity, CancellationToken token = default)
        {
            Task.Factory.StartNew( async () =>
            {
                await _context.Products.AddAsync(entity, token);
            }, token);
        }

        public void Update(Product entity, CancellationToken token = default)
        {
            _context.Products.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<int> BulkInsertAsync(IEnumerable<Product> products, CancellationToken token)
        {
            var productList = products.ToList();
            await _context.Products.AddRangeAsync(productList, token);

            return productList.Count;
        }

        public async Task<IEnumerable<Product>> TTSearchAsync(string query, CancellationToken token)
        {
            return await _context.Products.Where(x => x.SearchVector.Matches(query)).ToListAsync(token);
        }

        public async Task<Product> GetByNumberAsync(long number)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.GlobalNumber == number);
        }

        public async Task AddPurchaseForProductAsync(Guid id, decimal price, decimal originalPrice, Guid sellerId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                throw new InvalidOperationException($"product {id} is not exist");
            }

            var seller = await _context.Companies.FirstOrDefaultAsync(x => x.Id == sellerId);
            if (seller == null)
            {
                throw new InvalidOperationException($"seller {sellerId} is not exist");
            }

            await _context.SellerPrices.AddAsync(new ProductSellerPrice
            {
                Id = Guid.NewGuid(),
                Price = price,
                OriginalPrice = originalPrice,
                Product = product,
                ProductId = product.Id,
                Seller = seller,
                SellerId = seller.Id,
                OriginalCurrencyNumber = 643 //hardcoded currency
            });
        }
    }
}