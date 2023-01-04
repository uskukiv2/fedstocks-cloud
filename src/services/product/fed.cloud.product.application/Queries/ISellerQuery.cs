using fed.cloud.product.application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Queries;

public interface ISellerQuery
{
    Task<IEnumerable<SellerSummaryDto>> GetSellersAsync(Guid countryId, int countyNumber);
}