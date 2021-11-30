using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fed.cloud.product.application.Models;

namespace fed.cloud.product.application.Queries;

public interface ISellerQuery
{
    Task<IEnumerable<SellerSummaryDto>> GetSellersAsync(Guid countryId, int countyNumber);
}