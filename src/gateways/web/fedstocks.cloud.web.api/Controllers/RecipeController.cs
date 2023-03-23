using System.Net.Mime;
using fed.cloud.communication.Recipe;
using fed.cloud.menu.api.Protos;
using fedstocks.cloud.web.api.Helpers;
using FluentValidation;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtoRecipe = fed.cloud.menu.api.Protos.Recipe;
using Recipe = fed.cloud.communication.Recipe.Recipe;

namespace fedstocks.cloud.web.api.Controllers;

[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[Authorize(Policy = "menu")]
[ApiController]
public class RecipeController : ControllerBase
{
    private readonly ProtoRecipe.RecipeClient _recipeClient;
    private readonly IMapper _mapper;
    private readonly IValidator<Recipe> _recipeValidator;
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(ProtoRecipe.RecipeClient recipeClient, IMapper mapper, IValidator<Recipe> recipeValidator,
        ILogger<RecipeController> logger)
    {
        _recipeClient = recipeClient;
        _mapper = mapper;
        _recipeValidator = recipeValidator;
        _logger = logger;
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK), ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesAsync([FromQuery] int size, [FromQuery] int next)
    {
        var response = await _recipeClient.GetRecipesAsync(new RecipesRequest
        {
            PageSize = size,
            Skip = next
        });

        if (response is not null)
        {
            return Ok(_mapper.Map<IEnumerable<Recipe>>(response));
        }

        return NotFound();
    }

    [HttpPost]
    [ProducesDefaultResponseType, ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Recipe>> CreateRecipeAsync([FromBody] Recipe recipe)
    {
        var validate = await _recipeValidator.ValidateAsync(recipe);
        if (!validate.IsValid)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed, validate.Errors);
        }

        if (recipe.Id != Guid.Empty)
        {
            return StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        var response = await _recipeClient.AddOrUpdateRecipeAsync(new UpdateRecipeRequest()
        {
            Recipe = RecreateRecipeModelFromDto(recipe)
        });

        if (response is not null)
        {
            return Ok(_mapper.Map<Recipe>(response.Recipe));
        }

        return Conflict("Not possible to create recipe");
    }

    [HttpPatch]
    [ProducesDefaultResponseType, ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Recipe>> UpdateRecipeAsync([FromBody] Recipe recipe)
    {
        var response = await _recipeClient.AddOrUpdateRecipeAsync(new UpdateRecipeRequest()
        {
            Recipe = RecreateRecipeModelFromDto(recipe)
        });

        if (response is not null)
        {
            return Ok(_mapper.Map<Recipe>(response.Recipe));
        }

        return Conflict("Not possible no update recipe");
    }

    [HttpDelete]
    [ProducesDefaultResponseType, ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRecipeAsync([FromBody] Guid recipeId)
    {
        var response = await _recipeClient.DeleteRecipeAsync(new DeleteRecipeRequest()
        {
            Id = recipeId.ToString()
        });

        if (response is not null)
        {
            return Ok();
        }

        return NotFound();
    }

    private static RecipeModel RecreateRecipeModelFromDto(Recipe recipe)
    {
        var recipeModel = new RecipeModel
        {
            Id = recipe.Id.ToString("D"),
            Name = recipe.RecipeName,
            Tags = string.Join(';', recipe.Tags),
            CookingTime = recipe.CookingTime.ToDuration(),
            Content = recipe.PackContent()
        };

        recipeModel.RecipeIngredients.AddRange(recipe.Ingredients.Select(CreateRecipeIngredient));

        return recipeModel;
    }

    private static RecipeIngredientModel CreateRecipeIngredient(RecipeIngredient ingredient)
    {
        return new RecipeIngredientModel
        {
            Name = ingredient.IngredientName,
            Quantity = ingredient.Quantity,
            ProductNumber = ingredient.ReferenceNumber,
            UnitNumber = ingredient.Unit
        };
    }
}