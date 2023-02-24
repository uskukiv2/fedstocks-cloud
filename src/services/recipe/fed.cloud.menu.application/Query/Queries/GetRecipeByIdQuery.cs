using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Models.Read;

namespace fed.cloud.recipe.application.Query.Queries;

public class GetRecipeByIdQuery : IQuery<RecipeModel>
{
    public GetRecipeByIdQuery(Guid recipeId)
    {
        RecipeId = recipeId;
    }
    
    public Guid RecipeId { get; }
}

public class GetRecipeByIdQueryHandler : IQueryHandler<GetRecipeByIdQuery, RecipeModel>
{
    private readonly IFetchManager _fetchManager;

    public GetRecipeByIdQueryHandler(IFetchManager fetchManager)
    {
        _fetchManager = fetchManager;
    }
    
    public async Task<RecipeModel?> Handle(GetRecipeByIdQuery query)
    {
        var recipe = await _fetchManager.GetOneAsync<RecipeModel>(r => r.Id == query.RecipeId);

        recipe.Ingredients = await _fetchManager.GetAsync<RecipeIngredientModel>(r => r.RecipeId == recipe.Id);
        
        return recipe;
    }
} 