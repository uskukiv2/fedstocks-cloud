using fed.cloud.communication.Seller;
using fed.cloud.product.host.Protos;
using fedstocks.cloud.web.api.Helpers;
using RemoteSeller = fed.cloud.product.host.Protos.Seller;

namespace fedstocks.cloud.web.api.Services.Implementation;

public class SellerService : ISellerService
{
    private readonly RemoteSeller.SellerClient _client;
    private readonly ILogger<SellerService> _logger;

    public SellerService(RemoteSeller.SellerClient client, ILogger<SellerService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<SellerSummary>> SearchSellerAsync(string query, Guid county)
    {
        _logger.LogTrace("sending request to seller - search seller");
        var request = new SellerQueryRequest
        {
            Query = query,
            County = county.ToString()
        };
        try
        {
            var response = await _client.QuerySellersAsync(request, deadline: DateTime.UtcNow.AddSeconds(10));
            if (response == null)
            {
                _logger.LogDebug("search seller - respond with null");
                return new List<SellerSummary>();
            }

            _logger.LogTrace("search seller  - respond with {0}", response.ToString());
            return response.Sellers.Select(MapToDto);
        }
        catch (Exception ex)
        {
            return ExceptionHelper.HandleExceptionWithGrpc<List<SellerSummary>>(_logger, ex);
        }
    }

    private static SellerSummary MapToDto(SellerData arg)
    {
        return new SellerSummary
        {
            Id = Guid.Parse(arg.Id),
            Name = arg.Name,
        };
    }
}