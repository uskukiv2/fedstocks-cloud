using System.Reflection.Metadata;
using fed.cloud.product.application.Commands;
using fed.cloud.product.application.Models;
using fed.cloud.product.application.Queries;
using fed.cloud.product.host.Protos;
using Grpc.Core;
using MediatR;

namespace fed.cloud.product.host.Services;

public class CountryService : Country.CountryBase
{
    private readonly IMediator _mediator;
    private readonly ICountryQuery _countryQuery;
    private readonly ILogger<CountryService> _logger;

    public CountryService(IMediator mediator, ICountryQuery countryQuery, ILogger<CountryService> logger)
    {
        _mediator = mediator;
        _countryQuery = countryQuery;
        _logger = logger;
    }

    public override async Task<CountyResponse> GetCounties(CountyRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.CountryId, out var countryId) && countryId == Guid.Empty)
        {
            _logger.LogWarning("Could not parse given country id {countryId}", countryId);
            context.Status = Status.DefaultCancelled;
            return new CountyResponse();
        }

        try
        {
            var counties = (await _countryQuery.GetCountyByCountryAsync(countryId)).ToList();
            if (counties.Any())
            {
                context.Status = Status.DefaultSuccess;
                return MapToResponse(counties);
            }

            context.Status = new Status(StatusCode.NotFound, $"could not find counties for country {countryId}");
            return new CountyResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{message} caught an error while getting counties for {country}", e.Message, countryId);
            context.Status = new Status(StatusCode.ResourceExhausted, "could not find counties", e);
            return new CountyResponse();
        }
    }

    public override async Task<CountriesResultResponse> QueryCountries(CountryQueryRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Query))
        {
            context.Status = new Status(StatusCode.InvalidArgument, "query is not given");
            return new CountriesResultResponse();
        }

        var command = new HandleCountriesRequestQueryCommand(request.Query);
        var result = (await _mediator.Send(command)).ToList();
        if (result.Any())
        {
            context.Status = Status.DefaultSuccess;
            return MapToResponse(result);
        }

        context.Status = Status.DefaultCancelled;
        return new CountriesResultResponse();
    }

    private static CountriesResultResponse MapToResponse(IEnumerable<CountryDto> counties)
    {
        var response = new CountriesResultResponse();
        foreach (var country in counties)
        {
            response.Items.Add(MapToResponseData(country));
        }

        return response;
    }

    private static CountyResponse MapToResponse(IEnumerable<CountyDto> counties)
    {
        var response = new CountyResponse();
        foreach (var countyDto in counties)
        {
            response.Items.Add(MapToResponseData(countyDto));
        }

        return response;
    }

    private static CountyMessageData MapToResponseData(CountyDto countyDto)
    {
        return new CountyMessageData
        {
            Id = countyDto.NumberInCountry,
            Name = countyDto.Name
        };
    }

    private static CountryMessageData MapToResponseData(CountryDto countryDto)
    {
        return new CountryMessageData
        {
            Id = countryDto.Id.ToString(),
            Name = countryDto.Name
        };
    }
}