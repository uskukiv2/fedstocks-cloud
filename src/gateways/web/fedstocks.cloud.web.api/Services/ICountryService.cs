using fedstocks.cloud.web.api.Models;

namespace fedstocks.cloud.web.api.Services;

public interface ICountryService
{
    Task<IEnumerable<CountrySummary>> SearchCountriesAsync(string query);
    Task<Country> GetCountryAsync(Guid countryId);
}