using System.Net.Mime;
using System.Text;
using fedstocks.cloud.web.api.Models;
using fedstocks.cloud.web.api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace fedstocks.cloud.web.api.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidatorFactory _validatorFactory;
    private readonly IIdentityService _identityService;

    public ProductController(IProductService productService, IValidatorFactory validatorFactory,
        IIdentityService identityService)
    {
        _productService = productService;
        _validatorFactory = validatorFactory;
        _identityService = identityService;
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<ProductSummary>>> GetProducts([FromQuery] string query)
    {
        if (_identityService.GetUserSub(HttpContext) == Guid.Empty)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(query))
        {
            return Forbid();
        }

        var result = await _productService.SearchProductsAsync(query);
        if (result == null)
        {
            return StatusCode(503, "product service is unavailable");
        }
        if (!result.Any())
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("get/{product:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> GetProduct(long product, [FromQuery]Guid seller)
    {
        if (_identityService.GetUserSub(HttpContext) == Guid.Empty)
        {
            return Unauthorized();
        }

        if (product <= 0 || seller == Guid.Empty)
        {
            return Forbid();
        }

        var productResult = await _productService.GetProductByNumberAsync(product);

        var validator = _validatorFactory.GetValidator<Product>();
        var validationResults = await validator.ValidateAsync(productResult);
        if (validationResults.IsValid)
        {
            return Ok(productResult);
        }

        var errorsStringBuilder = new StringBuilder();
        errorsStringBuilder.AppendLine();
        validationResults.Errors.ForEach(x =>
            errorsStringBuilder.AppendLine(
                (string?) $"{x.PropertyName} - {x.ErrorCode} {x.ErrorMessage} with {x.AttemptedValue}"));

        return BadRequest(errorsStringBuilder.ToString());
    }
}