using System.Runtime.InteropServices;
using fed.cloud.product.host.Protos;
using fedstocks.cloud.web.api.Helpers;
using fedstocks.cloud.web.api.Models;
using Grpc.Core;
using Country = fedstocks.cloud.web.api.Models.Country;
using RemoteCountry = fed.cloud.product.host.Protos.Country;

namespace fedstocks.cloud.web.api.Services.Implementation;

public class CountryService : ICountryService
{
    private readonly RemoteCountry.CountryClient _client;
    private readonly ILogger<CountryService> _logger;

    public CountryService(RemoteCountry.CountryClient client, ILogger<CountryService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<CountrySummary>> SearchCountriesAsync(string query)
    {
        _logger.LogTrace("sending request to country - search by query");
        var request = new CountryQueryRequest
        {
            Query = query
        };

        try
        {
            var response = await _client.QueryCountriesAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response == null || !response.Items.Any())
            {
                _logger.LogDebug("search by query countries - respond with null");
                return new List<CountrySummary>();
            }

            _logger.LogTrace("search by query countries - respond with total {0}", response.Items.Count);
            return response.Items.Select(MapToDto);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<List<CountrySummary>>(_logger, ex);
        }
    }

    public async Task<Country> GetCountryAsync(Guid countryId)
    {
        _logger.LogTrace("sending request to country - get country");
        var request = new CountyRequest
        {
            CountryId = countryId.ToString()
        };

        try
        {
            var response = await _client.GetCountiesAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response == null || !response.Items.Any())
            {
                _logger.LogDebug("get country - respond with null");
                return new Country();
            }

            _logger.LogTrace("get country - respond with {0} total {1}", string.Empty, response.ToString());
            return MapToDto(response);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<Country>(_logger, ex);
        }
    }

    private static Country MapToDto(CountyResponse response)
    {
        return new Country
        {
            Id = Guid.Parse(response.Country.Id),
            Name = response.Country.Name,
            Counties = response.Items.Select(MapToDto)
        };
    }

    private static CountrySummary MapToDto(CountryMessageData arg)
    {
        return new CountrySummary
        {
            Id = Guid.Parse(arg.Id),
            Name = arg.Name
        };
    }

    private static County MapToDto(CountyMessageData data)
    {
        return new County
        {
            Id = Guid.Parse(data.Id),
            Number = data.Number,
            Name = data.Name
        };
    }
}