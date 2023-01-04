using fed.cloud.communication.Seller;

namespace fedstocks.cloud.web.api.Services;

public interface ISellerService
{
    Task<IEnumerable<SellerSummary>> SearchSellerAsync(string query, Guid county);
}