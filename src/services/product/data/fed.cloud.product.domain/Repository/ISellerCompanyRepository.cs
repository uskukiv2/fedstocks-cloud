using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.domain.Repository
{
    public interface ISellerCompanyRepository : IRepository<SellerCompany>
    {
        Task<bool> IsCompanyExistsAsync(Guid sellerId);
        Task<IEnumerable<SellerCompany>> TTSearchAsync(string query, Guid countryId, CancellationToken token);
        Task<IEnumerable<SellerCompany>> TTSearchAsync(string query, Guid countryId, Guid countyId, CancellationToken token);
    }
}
