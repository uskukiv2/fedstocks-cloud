using fed.cloud.product.application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Queries
{
    public interface IProductQuery
    {
        Task<ProductDto> GetProductByNumberAsync(long number);
    }
}
