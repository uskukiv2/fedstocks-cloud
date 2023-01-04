using fed.cloud.communication.Country;
using fedstocks.cloud.web.api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;

namespace fedstocks.cloud.web.api.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Authorize(Policy = "jwt")]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IIdentityService _identityService;
        private readonly IValidatorFactory _validatorFactory;

        public CountryController(ICountryService countryService, IIdentityService identityService,
            IValidatorFactory validatorFactory)
        {
            _countryService = countryService;
            _identityService = identityService;
            _validatorFactory = validatorFactory;
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CountrySummary>>> Search([FromQuery] string query)
        {
            var userId = _identityService.GetUserSub(HttpContext);
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(query))
            {
                return Forbid();
            }

            var countryResults = await _countryService.SearchCountriesAsync(query);
            if (countryResults == null || !countryResults.Any())
            {
                return NotFound();
            }

            return Ok(countryResults);
        }

        [HttpGet("get/{country:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Country>> GetCountry(Guid country)
        {
            if (_identityService.GetUserSub(HttpContext) == Guid.Empty)
            {
                return Unauthorized();
            }

            if (country == Guid.Empty)
            {
                return Forbid();
            }

            var countryResult = await _countryService.GetCountryAsync(country);
            var validator = _validatorFactory.GetValidator<Country>();
            var validationResults = await validator.ValidateAsync(countryResult);
            if (validationResults.IsValid)
            {
                return Ok(countryResult);
            }

            var errorsStringBuilder = new StringBuilder();
            errorsStringBuilder.AppendLine();
            validationResults.Errors.ForEach(x =>
                errorsStringBuilder.AppendLine(
                    (string?)$"{x.PropertyName} - {x.ErrorCode} {x.ErrorMessage} with {x.AttemptedValue}"));

            return BadRequest(errorsStringBuilder.ToString());
        }
    }
}
