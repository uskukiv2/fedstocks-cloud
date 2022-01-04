using fedstocks.cloud.web.api.Models;

namespace fedstocks.cloud.web.api.Services;

public interface IProductService
{
    Task<IEnumerable<ProductSummary>> SearchProductsAsync(string searchQuery);
    Task<Product> GetProductByNumberAsync(long number);
}