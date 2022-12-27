using fed.cloud.product.application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Queries;

public interface ICountryQuery
{
    Task<IEnumerable<CountryDto>> GetCountriesAsync(int top = 10);

    Task<CountryDto> GetCountryByIdAsync(Guid countryId);
}