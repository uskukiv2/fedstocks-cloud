using System.Net.Mime;
using System.Text;
using fed.cloud.communication.Shopper;
using fedstocks.cloud.web.api.Models;
using fedstocks.cloud.web.api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fedstocks.cloud.web.api.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "jwt")]
    public class ShoppingController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IShoppingService _shoppingService;
        private readonly IValidatorFactory _validatorFactory;

        public ShoppingController(IIdentityService identityService, IShoppingService shoppingService,
            IValidatorFactory validatorFactory)
        {
            _identityService = identityService;
            _shoppingService = shoppingService;
            _validatorFactory = validatorFactory;
        }

        [HttpPost("new")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CompletedShoppingList>> NewList([FromBody] NewShoppingList list)
        {
            var userId = _identityService.GetUserSub(HttpContext);
            if (userId == Guid.Empty)
            {
                return Unauthorized("unauthorized user");
            }
            var shoppingListValidator = _validatorFactory.GetValidator<NewShoppingList>();
            var validationResults = await shoppingListValidator.ValidateAsync(list);
            if (!validationResults.IsValid)
            {
                var errorsStringBuilder = new StringBuilder();
                errorsStringBuilder.AppendLine();
                validationResults.Errors.ForEach(x =>
                    errorsStringBuilder.AppendLine(
                        $"{x.PropertyName} - {x.ErrorCode} {x.ErrorMessage} with {x.AttemptedValue}"));

                return BadRequest(errorsStringBuilder.ToString());
            }

            var shoppingList = await _shoppingService.CreateShoppingListAsync(list, userId);
            if (shoppingList.Id == -1)
            {
                return BadRequest();
            }
            return Created(string.Empty, shoppingList);
        }

        [HttpPost("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CompletedShoppingList>> UpdateList([FromBody] CompletedShoppingList list)
        {
            var userId = _identityService.GetUserSub(HttpContext);
            if (userId == Guid.Empty)
            {
                return Unauthorized("unauthorized user");
            }

            var validationsResult = await (_validatorFactory.GetValidator<CompletedShoppingList>()).ValidateAsync(list);
            if (!validationsResult.IsValid)
            {
                var errorsStringBuilder = new StringBuilder();
                errorsStringBuilder.AppendLine();
                validationsResult.Errors.ForEach(x =>
                    errorsStringBuilder.AppendLine(
                        $"{x.PropertyName} - {x.ErrorCode} {x.ErrorMessage} with {x.AttemptedValue}"));

                return BadRequest(errorsStringBuilder.ToString());
            }

            var updatedShoppingList = await _shoppingService.UpdateShoppingListAsync(list, userId);
            if (updatedShoppingList.Id == -1)
            {
                return NotFound();
            }

            return Ok(updatedShoppingList);
        }

        [HttpPatch("checkout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ShoppingCheckoutResult>> CheckoutList([FromBody] ShoppingCheckoutRequest request)
        {
            var userId = _identityService.GetUserSub(HttpContext);
            if (userId == Guid.Empty)
            {
                return Unauthorized("unauthorized user");
            }

            if (request.ShoppingListId <= 0)
            {
                return BadRequest("id is not given");
            }

            var checkoutResult = await _shoppingService.CheckoutShoppingListAsync(userId, request.ShoppingListId);
            if (checkoutResult == null || checkoutResult.ShoppingId == -1)
            {
                return NotFound();
            }

            if (checkoutResult.TotalLines == -1)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    $"For {checkoutResult.ShoppingId}{checkoutResult.Name} is not all lines is checked or checkout is not forced");
            }

            return Ok(checkoutResult);
        }

        //TODO: FED-124 Add ability to remove shopping list
        [HttpDelete("remove")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>>RemoveList()
        {
            return Forbid();
        }

        [HttpGet("get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public async Task<ActionResult<IEnumerable<CompletedShoppingList>>> GetLists()
        {
            var userId = _identityService.GetUserSub(HttpContext);
            if (userId == Guid.Empty)
            {
                return Unauthorized("unauthorized user");
            }

            var lists = await _shoppingService.GetShoppingListsAsync(userId);
            if (lists == null)
            {
                return Problem(statusCode:405);
            }
            if (!lists.Any())
            {
                return NotFound();
            }

            return Ok(lists);
        }

        [HttpGet("get/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CompletedShoppingList>> GetList(int id)
        {
            var userId = _identityService.GetUserSub(HttpContext);
            if (userId == Guid.Empty)
            {
                return Unauthorized("unauthorized user");
            }

            if (id <= 0)
            {
                return NoContent();
            }

            var list = await _shoppingService.GetShoppingListAsync(userId, id);
            if (list.Id <= 0)
            {
                return NotFound();
            }

            return Ok(list);
        }
    }
}
