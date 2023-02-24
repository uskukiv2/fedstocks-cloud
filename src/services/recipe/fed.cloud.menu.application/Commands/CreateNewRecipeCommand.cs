using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Factories;
using fed.cloud.menu.data.Repository;
using fed.cloud.recipe.application.Abstract;
using gen.common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fed.cloud.recipe.application.Commands;

public class CreateNewRecipeCommand : RequestBase<Guid>
{
    public CreateNewRecipeCommand(Guid userId, Recipe recipe) : base(userId)
    {
        RecipeToCreate = recipe;
    }
    
    public Recipe RecipeToCreate { get; }
}

public class CreateNewRecipeCommandHandler : IRequestHandler<CreateNewRecipeCommand, Guid>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ILogger<CreateNewRecipeCommandHandler> _logger;

    public CreateNewRecipeCommandHandler(IRecipeRepository recipeRepository,
        ILogger<CreateNewRecipeCommandHandler> logger)
    {
        _recipeRepository = recipeRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateNewRecipeCommand request, CancellationToken cancellationToken)
    {
        var newRecipeId = Guid.NewGuid();
        await Task.Run(() =>
        {
            var recipe = request.RecipeToCreate;
            recipe.Id = newRecipeId;
            recipe.OwnerId = request.UserId;
            _recipeRepository.Add(recipe, cancellationToken);
        }, cancellationToken).Forget();

        return newRecipeId;
    }
}