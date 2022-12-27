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

public class HandleSellersRequestQueryCommand : IRequest<IEnumerable<SellerSummaryDto>>
{
    public HandleSellersRequestQueryCommand(string query, Guid country)
    {
        Query = query;
        County = country;
    }

    public string Query { get; }

    public Guid County { get; }
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
        if (request.County == Guid.Empty)
        {
            return new List<SellerSummaryDto>();
        }

        try
        {
            var sellers = await _sellerCompanyRepository.TTSearchAsync(request.Query, request.County, cancellationToken);

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
