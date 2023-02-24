using gen.fedstocks.web.Client.Application.Models.Products;

namespace gen.fedstocks.web.Client.Application.Services;

public interface IProductService
{
    public Task<IEnumerable<UnitDto>> GetAvailableUnitsAsync();
}