using fed.cloud.product.application.Models;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Commands;

public class HandleCountriesRequestQueryCommand : IRequest<IEnumerable<CountryDto>>
{
    public HandleCountriesRequestQueryCommand(string query)
    {
        Query = query;
    }
    public string Query { get; }
}

public class HandleCountriesRequestQueryCommandHandler : IRequestHandler<HandleCountriesRequestQueryCommand, IEnumerable<CountryDto>>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<HandleCountriesRequestQueryCommandHandler> _logger;

    public HandleCountriesRequestQueryCommandHandler(ICountryRepository countryRepository, ILogger<HandleCountriesRequestQueryCommandHandler> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CountryDto>> Handle(HandleCountriesRequestQueryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _countryRepository.TTSearchAsync(request.Query, cancellationToken);

            return result.Select(MapToDto);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "{message} caught an error while execution query {query}", e.Message, request.Query);
            return new List<CountryDto>();
        }
    }

    private static CountryDto MapToDto(Country arg)
    {
        return new CountryDto
        {
            Id = arg.Id,
            Name = arg.Name,
        };
    }
}
