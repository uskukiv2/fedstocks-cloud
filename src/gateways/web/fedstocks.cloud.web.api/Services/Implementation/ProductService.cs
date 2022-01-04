using fed.cloud.product.host.Protos;
using fedstocks.cloud.web.api.Helpers;
using fedstocks.cloud.web.api.Models;
using Grpc.Core;
using Newtonsoft.Json;
using Product = fedstocks.cloud.web.api.Models.Product;
using ProductSummary = fedstocks.cloud.web.api.Models.ProductSummary;
using RemoteProduct = fed.cloud.product.host.Protos.Product;

namespace fedstocks.cloud.web.api.Services.Implementation;

public class ProductService : IProductService
{
    private readonly RemoteProduct.ProductClient _client;
    private readonly ILogger<ProductService> _logger;

    public ProductService(RemoteProduct.ProductClient client, ILogger<ProductService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductSummary>> SearchProductsAsync(string searchQuery)
    {
        _logger.LogTrace("sending request to product - search product by query");
        var request = new RequestProducts
        {
            Query = searchQuery
        };
        try
        {
            var response = await _client.QueryProductAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response is null)
            {
                _logger.LogDebug("search product - respond with null");
                return new List<ProductSummary>();
            }

            _logger.LogTrace("search product - respond with total {0}", response.Items.Count);
            return response.Items.Select(MapToDto);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<List<ProductSummary>>(_logger, ex);
        }
    }

    public async Task<Product> GetProductByNumberAsync(long number)
    {
        _logger.LogTrace("sending request to product - get product");
        var request = new RequestProduct
        {
            Number = number
        };
        try
        {
            var response = await _client.GetProductAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response == null)
            {
                _logger.LogDebug("search product - respond with null");
                return new Product();
            }

            _logger.LogTrace("get product - respond with total name: {0} ", response.Name);
            return MapToDto(response);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<Product>(_logger, ex);
        }
    }

    private static ProductSummary MapToDto(fed.cloud.product.host.Protos.ProductSummary arg)
    {
        return new ProductSummary
        {
            Name = $"({arg.Brand}) {arg.Name}",
            Number = arg.Number
        };
    }

    //TODO FED-127
    private static Product MapToDto(ProductData response)
    {
        return new Product
        {
            Brand = response.Brand,
            Name = response.Name,
            Number = response.Number
        };
    }
}