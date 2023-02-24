using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using fed.cloud.recipe.application.Abstract;
using fed.cloud.recipe.application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.recipe.application.Commands;

public class UpdateRecipeCommand : RequestBase<Recipe>
{
    public UpdateRecipeCommand(Guid userId, Recipe recipeDto) : base(userId)
    {
        RecipeToUpdate = recipeDto;
    }
    
    public Recipe RecipeToUpdate { get; }
}

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, Recipe>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<UpdateRecipeCommandHandler> _logger;

    public UpdateRecipeCommandHandler(IRecipeRepository recipeRepository, ILogger<UpdateRecipeCommandHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }
    
    public async Task<Recipe> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetAsync(request.RecipeToUpdate.Id);
        if (recipe.OwnerId != request.UserId)
        {
            throw new UserCannotUpdateRecipeException(request.UserId, request.RecipeToUpdate.Id);
        }
        
        _recipeRepository.Update(recipe, cancellationToken);

        return recipe;
    }
}