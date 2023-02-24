using fed.cloud.menu.data.Repository;
using fed.cloud.recipe.application.Abstract;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.recipe.application.Commands;

public class DeleteRecipeCommand : RequestBase<bool>
{
    public DeleteRecipeCommand(Guid userId, Guid recipeId) : base(userId)
    {
        RecipeId = recipeId;
    }

    public Guid RecipeId { get; }
}

public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand, bool>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<DeleteRecipeCommandHandler> _logger;

    public DeleteRecipeCommandHandler(IRecipeRepository recipeRepository, ILogger<DeleteRecipeCommandHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe =  await _recipeRepository.GetAsync(request.RecipeId);

        if (recipe.OwnerId != request.UserId)
        {
            _logger.LogTrace("Is not possible to remove recipe. User don't have rights");
            return false;
        }

        await _recipeRepository.DeleteRecipe(recipe.Id, cancellationToken);

        return true;
    }
} 