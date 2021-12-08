using fed.cloud.product.application.Commands;
using fed.cloud.product.application.Models;
using fed.cloud.product.application.Queries;
using fed.cloud.product.host.Protos;
using Grpc.Core;
using MediatR;

namespace fed.cloud.product.host.Services;

public class SellerService : Seller.SellerBase
{
    private readonly IMediator _mediator;
    private readonly ISellerQuery _sellerQuery;
    private readonly ILogger<SellerService> _logger;

    public SellerService(IMediator mediator, ISellerQuery sellerQuery, ILogger<SellerService> logger)
    {
        _mediator = mediator;
        _sellerQuery = sellerQuery;
        _logger = logger;
    }

    public override async Task<SellersResultResponse> QuerySellers(SellerQueryRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Query))
        {
            context.Status = Status.DefaultCancelled;
            return new SellersResultResponse();
        }

        if (!Guid.TryParse(request.CountryId, out var countryId))
        {
            context.Status = Status.DefaultCancelled;
            return new SellersResultResponse();
        }

        var command = new HandleSellersRequestQueryCommand(request.Query, countryId, request.County);
        var result = (await _mediator.Send(command, context.CancellationToken)).ToList();
        if (result.Any())
        {
            return MapToResponse(result);
        }

        context.Status = new Status(StatusCode.NotFound, "could not find any available sellers");
        return new SellersResultResponse();
    }

    public override async Task<SellersResultResponse> GetSellersByCountry(SellerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.CountryId, out var countryId) && countryId == Guid.Empty)
        {
            context.Status = Status.DefaultCancelled;
            return new SellersResultResponse();
        }

        try
        {
            var result = (await _sellerQuery.GetSellersAsync(countryId, request.County)).ToList();
            if (result.Any())
            {
                context.Status = new Status(StatusCode.OK, string.Empty);
                return MapToResponse(result);
            }

            context.Status = new Status(StatusCode.NotFound, "cannot found any sellers for given country and county");
            return new SellersResultResponse();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            context.Status = new Status(StatusCode.ResourceExhausted, "error caught while getting", e);
            return new SellersResultResponse();
        }
    }

    private static SellersResultResponse MapToResponse(IEnumerable<SellerSummaryDto> result)
    {
        var response = new SellersResultResponse();
        foreach (var sellerData in result.Select(MapToSellerData))
        {
            response.Sellers.Add(sellerData);
        }

        return response;
    }

    private static SellerData MapToSellerData(SellerSummaryDto arg)
    {
        return new SellerData
        {
            Id = arg.Id.ToString(),
            Name = arg.Name
        };
    }
}