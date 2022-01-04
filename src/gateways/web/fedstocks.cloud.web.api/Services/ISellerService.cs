using fedstocks.cloud.web.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace fedstocks.cloud.web.api.Services;

public interface ISellerService
{
    Task<IEnumerable<SellerSummary>> SearchSellerAsync(string query, Guid county);
}