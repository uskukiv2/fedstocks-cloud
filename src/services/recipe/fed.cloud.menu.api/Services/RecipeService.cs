using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Models.Read;
using fed.cloud.recipe.application.Abstract;
using fed.cloud.recipe.application.Commands;
using fed.cloud.recipe.application.Query.Queries;
using gen.common.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MapsterMapper;
using MediatR;

using Proto = fed.cloud.menu.api.Protos;

#nullable disable

namespace fed.cloud.menu.api.Services;

public class RecipeService : Proto.Recipe.RecipeBase
{
    private readonly IMapper _mapper;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IMediator _mediator;

    public RecipeService(IMapper mapper, IQueryProcessor queryProcessor, IMediator mediator)
    {
        _mapper = mapper;
        _queryProcessor = queryProcessor;
        _mediator = mediator;
    }

    public override async Task<Proto.RecipesResponse> GetRecipes(Proto.RecipesRequest request, ServerCallContext context)
    {
        try
        {
            var authId = GetAuthIdFromContext(context);
            if (string.IsNullOrEmpty(authId))
            {
                context.Status = new Status(StatusCode.Unauthenticated, "user cannot be validated");
                return new Proto.RecipesResponse();
            }

            var userId = await GetUserIdAndCreateIfNeededAsync(authId);
            var recipes =
                await (await _queryProcessor.Process(new GetRecipesByUserQuery(userId, request.PageSize, request.Skip))).ToListAsync();
            if (recipes.Any())
            {
                return CreateResponse(recipes, request.Skip);
            }

            context.Status = new Status(StatusCode.NotFound, "can't find recipes for this user");
            return new Proto.RecipesResponse();
        }
        catch (Exception e)
        {
            context.Status = new Status(StatusCode.DeadlineExceeded, "unexpected exception", e);
            return new Proto.RecipesResponse();
        }
    }

    public override Task<Proto.RecipesResponse> GetRecipesByQuery(Proto.RecipesByQueryRequest request, ServerCallContext context)
    {
        return base.GetRecipesByQuery(request, context);
    }

    public override async Task<Proto.RecipeResponse> AddOrUpdateRecipe(Proto.UpdateRecipeRequest request, ServerCallContext context)
    {
        try
        {
            var authId = GetAuthIdFromContext(context);
            if (string.IsNullOrEmpty(authId))
            {
                context.Status = new Status(StatusCode.Unauthenticated, "user cannot be validated");
                return new Proto.RecipeResponse();
            }

            var userId = await GetUserIdAndCreateIfNeededAsync(authId);
            
            if (string.IsNullOrEmpty(request.Recipe.Id))
            {
                var newRecipeId = await _mediator.Send(new CreateNewRecipeCommand(userId, _mapper.Map<Recipe>(request.Recipe)));
                var newRecipe = await _queryProcessor.Process(new GetRecipeByIdQuery(newRecipeId));
                return new Proto.RecipeResponse
                {
                    Recipe = CreateRecipe(newRecipe)
                };
            }

            // incoming maybe existing recipe
            if (Guid.TryParse(request.Recipe.Id, out var recipeId))
            {
                await _mediator.Send(new UpdateRecipeCommand(userId, _mapper.Map<Recipe>(request.Recipe)));

                return new Proto.RecipeResponse
                {
                    Recipe = CreateRecipe(await _queryProcessor.Process(new GetRecipeByIdQuery(recipeId)))
                };
            }

            context.Status = new Status(StatusCode.NotFound, "unable to create or update recipe");
            return new Proto.RecipeResponse();
        }
        catch (Exception e)
        {
            context.Status = new Status(StatusCode.DeadlineExceeded, "unexpected exception", e);
            return new Proto.RecipeResponse();
        }
    }

    public override async Task<Empty> DeleteRecipe(Proto.DeleteRecipeRequest request, ServerCallContext context)
    {
        try
        {
            var authId = GetAuthIdFromContext(context);
            if (string.IsNullOrEmpty(authId))
            {
                context.Status = new Status(StatusCode.Unauthenticated, "user cannot be validated");
                return new Empty();
            }
    
            var userId = await GetUserIdAndCreateIfNeededAsync(authId);

            var isSuccess = await _mediator.Send(new DeleteRecipeCommand(userId, Guid.Parse(request.Id)));
            context.Status = isSuccess ? Status.DefaultSuccess : Status.DefaultCancelled;

            return new Empty();
        }
        catch (Exception e)
        {
            context.Status = new Status(StatusCode.DeadlineExceeded, "unexpected exception", e);
            return new Empty();
        }
    }

    private async Task<Guid> GetUserIdAndCreateIfNeededAsync(string authId)
    {
        var userId = Guid.NewGuid();
        var user = await _queryProcessor.Process(new GetUserByAuthIdQuery(authId));
        if (user == null)
        {
            await _mediator.Publish(new CreateUserNotificationCommand(userId, authId)).Forget();
        }
        else
        {
            userId = user.Id;
        }

        return userId;
    }

    private Proto.RecipesResponse CreateResponse(IEnumerable<RecipeModel> recipes, int previousSkip)
    {
        var recipeModels = recipes.ToList();
        var response =  new Proto.RecipesResponse
        {
            NextSkip = recipeModels.Count + previousSkip
        };
        
        response.Recipes.Clear();
        response.Recipes.AddRange(_mapper.Map<IEnumerable<Proto.RecipeModel>>(recipes));

        return response;
    }

    private static Proto.RecipeModel CreateRecipe(RecipeModel recipe)
    {
        var recipeModel = new Proto.RecipeModel
        {
            Id = recipe.Id.ToString("D"),
            Name = recipe.Name,
            Content = recipe.Content,
            Tags = recipe.Tags,
            CookingTime = recipe.CookingTime.ToDuration()
        };
        
        recipeModel.RecipeIngredients.AddRange(recipe.Ingredients.Select(CreateIngredient));

        return recipeModel;
    }

    private static Proto.RecipeIngredientModel CreateIngredient(RecipeIngredientModel arg)
    {
        return new Proto.RecipeIngredientModel
        {
            Name = arg.Name,
            Quantity = arg.Quantity,
            ProductNumber = arg.ProductNumber,
            UnitNumber = 0
        };
    }

    private static string? GetAuthIdFromContext(ServerCallContext context)
    {
        if (context.AuthContext.IsPeerAuthenticated)
        {
            return context.AuthContext.PeerIdentity
                .FirstOrDefault(x => x.Name == context.AuthContext.PeerIdentityPropertyName)
                ?.Value;
        }

        return string.Empty;
    }
}