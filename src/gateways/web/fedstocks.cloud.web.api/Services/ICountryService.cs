using fed.cloud.communication.Country;

namespace fedstocks.cloud.web.api.Services;

public interface ICountryService
{
    Task<IEnumerable<CountrySummary>> SearchCountriesAsync(string query);
    Task<Country> GetCountryAsync(Guid countryId);
}