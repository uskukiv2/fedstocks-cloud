using fed.cloud.product.application.Commands;
using fed.cloud.product.application.Models;
using fed.cloud.product.application.Queries;
using fed.cloud.product.host.Protos;
using Grpc.Core;
using MediatR;
using Unit = fed.cloud.product.host.Protos.Unit;

namespace fed.cloud.product.host.Services
{
    public class ProductService : Product.ProductBase
    {
        private readonly IMediator _mediator;
        private readonly IProductQuery _productQuery;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IMediator mediator, IProductQuery productQuery, ILogger<ProductService> logger)
        {
            _mediator = mediator;
            _productQuery = productQuery;
            _logger = logger;
        }

        public override async Task<ResultResponse> QueryProduct(RequestProducts request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Query))
            {
                context.Status = Status.DefaultCancelled;
                return new ResultResponse();
            }

            var command = new HandleProductsRequestQueryCommand(request.Query);
            var result = await _mediator.Send(command, context.CancellationToken);
            if (result == null)
            {
                return new ResultResponse();
            }
            var products = result.ToList();
            if (products.Any())
            {
                return GetResponse(products);
            }

            context.Status = new Status(StatusCode.NotFound, "cannot find any items with such pattern");
            return new ResultResponse();
        }

        public override async Task<ProductData> GetProduct(RequestProduct request, ServerCallContext context)
        {
            if (request.Number == 0)
            {
                context.Status = Status.DefaultCancelled;
                return new ProductData();
            }

            try
            {
                var product = await _productQuery.GetProductByNumberAsync(request.Number)!;
                if (product == null)
                {
                    context.Status = new Status(StatusCode.NotFound, $"cannot get product with number {request.Number}");
                    return new ProductData();
                }
                context.Status = Status.DefaultSuccess;
                return GetResponse(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Status = new Status(StatusCode.ResourceExhausted, ex.Message, ex);
                return new ProductData();
            }
        }

        private static ProductData GetResponse(ProductDto product)
        {
            var productData = new ProductData
            {
                Brand = product.Brand,
                Name = product.Title,
                Qty = product.DefaultQty,
                Category = MapToResponse(product.Category),
                Unit = MapToResponse(product.Unit),
                Number = product.Number
            };
            if (product.PriceDtos == null)
            {
                return productData;
            }

            foreach (var item in product.PriceDtos)
            {
                productData.Prices.Add(ToResponse(item));
            }

            return productData;

        }

        private static Unit MapToResponse(UnitDto unit)
        {
            return new Unit
            {
                Id = unit.Id,
                Name = unit.Name,
                Rate = unit.Rate
            };
        }

        private static Category MapToResponse(CategoryDto category)
        {
            return new Category
            {
                Id = category.Id,
                Name = category.Name,
                Parent = category.Parent != null ? MapToResponse(category.Parent) : new Category()
            };
        }

        private static SellerPrice ToResponse(ProductSellerPriceDto arg)
        {
            return new SellerPrice
            {
                Price = decimal.ToDouble(arg.Price),
                Currency = arg.CurrencyNumber
            };
        }

        private static ResultResponse GetResponse(IEnumerable<ProductSummaryDto> products)
        {
            var response = new ResultResponse();
            foreach (var item in products)
            {
                response.Items.Add(new ProductSummary()
                {
                    Brand = item.Brand,
                    Name = item.Title,
                    Number = item.Number
                });
            }

            return response;
        }
    }
}