using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using fed.cloud.product.application.Models;

namespace fed.cloud.product.application.Queries;

public interface ICountryQuery
{
    Task<IEnumerable<CountryDto>> GetCountriesAsync(int top = 10);

    Task<IEnumerable<CountyDto>> GetCountyByCountryAsync(Guid companyId);
}