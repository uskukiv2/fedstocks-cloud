﻿using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.domain.Repository
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<IEnumerable<Country>> TTSearchAsync(string query, CancellationToken token);
        Task<County?> GetCountyOfCountryAsync(Guid country, int countyNumber);
    }
}