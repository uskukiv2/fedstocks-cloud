using fed.cloud.communication.Seller;
using fedstocks.cloud.web.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace fedstocks.cloud.web.api.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[Authorize(Policy = "shopping")]
[ApiController]
public class SellerController : ControllerBase
{
    private readonly ISellerService _sellerService;
    private readonly IIdentityService _identityService;

    public SellerController(ISellerService sellerService, IIdentityService identityService)
    {
        _sellerService = sellerService;
        _identityService = identityService;
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<SellerSummary>>> GetSellers([FromQuery] Guid county, [FromQuery] string query)
    {
        if (_identityService.GetUserSub(HttpContext) == Guid.Empty)
        {
            return Unauthorized();
        }

        if (county == Guid.Empty || string.IsNullOrEmpty(query))
        {
            return Forbid();
        }

        var sellerResults = await _sellerService.SearchSellerAsync(query, county);
        if (!sellerResults.Any())
        {
            return NotFound();
        }

        return Ok(sellerResults);
    }
}