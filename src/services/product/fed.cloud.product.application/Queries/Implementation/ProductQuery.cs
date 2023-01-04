using fed.cloud.common.Infrastructure;
using fed.cloud.product.application.Models;
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
            var totalCategories = new List<dynamic>();
            var product = (await conn.QueryAsync("product.products", new { GlobalNumber = number.ToString() })).FirstOrDefault();
            if (product == null)
            {
                throw new InvalidOperationException($"Cannot get product with number {number}");
            }
            var productUnit = (await conn.QueryAsync("product.productunits", new { Id = product.UnitId })).FirstOrDefault();
            var nextCategoryId = product.CategoryId;
            while (true)
            {
                var category = (await conn.QueryAsync("product.productcategories", new { Id = nextCategoryId })).FirstOrDefault();
                totalCategories.Add(category);
                if (category.ParentId == null)
                {
                    break;
                }

                nextCategoryId = category.ParentId;
            }
            var productSellerPrices =
                (await conn.QueryAsync("product.productsellerprices", new { ProductId = product.Id },
                    Field.From("SellerId", "OriginalPrice", "OriginalCurrencyNumber"), top: 10)).ToList();

            return !productSellerPrices.Any() ? CreateProductDto(product, productUnit, totalCategories) : CreateProductDto(product, productUnit, totalCategories, productSellerPrices);
        }

        private static ProductDto CreateProductDto(dynamic product, dynamic productUnit, IEnumerable<dynamic> categories)
        {
            return new ProductDto
            {
                Brand = product.Brand,
                Title = product.Name,
                DefaultQty = product.QuantityRate,
                Number = long.Parse(product.GlobalNumber),
                Unit = MapToUnit(productUnit),
                Category = categories.Count() > 1 ? MapToOriginalCategory(categories.ToList()) : MapToCategoryDto(categories.FirstOrDefault())
            };
        }

        private static ProductDto CreateProductDto(dynamic product, dynamic productUnit, IEnumerable<dynamic> categories, IEnumerable<dynamic> sellerPrices)
        {
            return new ProductDto
            {
                Brand = product.Brand,
                Title = product.Name,
                DefaultQty = product.QuantityRate,
                Number = long.Parse(product.GlobalNumber),
                Unit = MapToUnit(productUnit),
                Category = categories.Count() > 1 ? MapToOriginalCategory(categories.ToList()) : MapToCategoryDto(categories.FirstOrDefault()),
                PriceDtos = sellerPrices.Select(ToSellerPriceKeyValue).ToArray()
            };
        }

        private static CategoryDto MapToOriginalCategory(IReadOnlyList<dynamic> categories)
        {
            CategoryDto originCategory = null!;
            var categoryParentId = new KeyValuePair<CategoryDto, int?>();
            for (var i = 0; i < categories.Count; i++)
            {
                CategoryDto dto = categoryParentId.Key ?? MapToCategoryDto(categories[i]);
                int lastParentId = categoryParentId.Value ?? categories[i].ParentId;
                if (lastParentId != null)
                {
                    var next = categories.FirstOrDefault(x => x.Id == lastParentId);
                    dto.Parent = MapToCategoryDto(next);
                    categoryParentId = new KeyValuePair<CategoryDto, int?>(dto.Parent, next.ParentId);
                    i++;
                }

                if (i == 0)
                {
                    originCategory = dto;
                }
            }

            return originCategory;
        }

        private static CategoryDto MapToCategoryDto(dynamic category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        private static UnitDto MapToUnit(dynamic productUnit)
        {
            return new UnitDto
            {
                Id = productUnit.Id,
                Name = productUnit.Name,
                Rate = productUnit.Rate
            };
        }

        private static ProductSellerPriceDto ToSellerPriceKeyValue(dynamic arg)
        {
            return new ProductSellerPriceDto
            {
                CurrencyNumber = arg.OriginalCurrencyNumber,
                Price = arg.OriginalPrice
            };
        }
    }
}
