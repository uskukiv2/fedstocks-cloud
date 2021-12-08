using fed.cloud.common.Infrastructure;
using fed.cloud.product.application.Models;
using fed.cloud.product.domain.Entities;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Queries.Implementation
{
    public class ProductQuery : IProductQuery
    {
        private readonly string _connection;

        public ProductQuery(IServiceConfiguration configuration)
        {
            _connection = configuration.Database.Connection;
        }

        public async Task<ProductDto> GetProductByNumberAsync(long number)
        {
            await using var conn = new NpgsqlConnection(_connection);

            var product = (await conn.QueryAsync("products", new { GlobalNumber = number },
                Field.From("GlobalNumber", "Brand", "Name", "CategoryId", "QuantityRate", "UnitId"))).FirstOrDefault();
            if (product == null)
            {
                throw new InvalidOperationException($"Cannot get product with number {number}");
            }
            var productSellerPrices =
                (await conn.QueryAsync("productSellerPrices", new {ProductId = product.Id},
                    Field.From("SellerId", "OriginalPrice", "OriginalCurrencyNumber"), top: 10)).ToList();

            return productSellerPrices.Any() ? CreateProductDto(product) : CreateProductDto(product, productSellerPrices);
        }

        private static ProductDto CreateProductDto(dynamic product)
        {
            return new ProductDto
            {
                Brand = product.Brand,
                Title = product.Name,
                Category = product.CategoryId,
                DefaultQty = product.QuantityRate,
                Number = product.GlobalNumber,
                Unit = product.UnitId,
            };
        }

        private static ProductDto CreateProductDto(Product product, IEnumerable<dynamic> sellerPrices)
        {
            return new ProductDto
            {
                Brand = product.Brand,
                Title = product.Name,
                Category = product.CategoryId,
                DefaultQty = product.QuantityRate,
                Number = product.GlobalNumber,
                Unit = product.UnitId,
                SellerPrices = sellerPrices.Select(ToSellerPriceKeyValue).ToArray()
            };
        }

        private static ProductSellerPriceDto ToSellerPriceKeyValue(dynamic arg)
        {
            return new ProductSellerPriceDto
            {
                SellerId = arg.SellerId,
                CurrencyNumber = arg.OriginalCurrencyNumber,
                Price = arg.OriginalPrice
            };
        }
    }
}
