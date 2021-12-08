using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.product.application.Models;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.product.application.Commands;

public class HandleSellersRequestQueryCommand : IRequest<IEnumerable<SellerSummaryDto>>
{
    public HandleSellersRequestQueryCommand(string query, Guid country, int county)
    {
        Query = query;
        Country = country;
        County = county;
    }

    public string Query { get; }

    public Guid Country { get; }

    public int County { get; }
}

public class HandleSellersRequestQueryCommandHandler : IRequestHandler<HandleSellersRequestQueryCommand, IEnumerable<SellerSummaryDto>>
{
    private readonly ISellerCompanyRepository _sellerCompanyRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<HandleSellersRequestQueryCommandHandler> _logger;

    public HandleSellersRequestQueryCommandHandler(ISellerCompanyRepository sellerCompanyRepository, ICountryRepository countryRepository, ILogger<HandleSellersRequestQueryCommandHandler> logger)
    {
        _sellerCompanyRepository = sellerCompanyRepository;
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SellerSummaryDto>> Handle(HandleSellersRequestQueryCommand request, CancellationToken cancellationToken)
    {
        if (request.Country == Guid.Empty)
        {
            return new List<SellerSummaryDto>();
        }

        try
        {
            if (request.County == 0)
            {
                _logger.LogInformation("Processing request for all counties of country {country}", request.Country);

                var sellers1 =
                    await _sellerCompanyRepository.TTSearchAsync(request.Query, request.Country, cancellationToken);

                return sellers1.Select(MapSellerSummaryDto);
            }

            var county = await _countryRepository.GetCountyOfCountryAsync(request.Country, request.County);
            var sellers = await _sellerCompanyRepository.TTSearchAsync(request.Query, request.Country, county.Id, cancellationToken);

            return sellers.Select(MapSellerSummaryDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return new List<SellerSummaryDto>();
        }
    }

    private static SellerSummaryDto MapSellerSummaryDto(SellerCompany arg)
    {
        return new SellerSummaryDto
        {
            Id = arg.Id,
            Name = arg.OriginalName
        };
    }
}
