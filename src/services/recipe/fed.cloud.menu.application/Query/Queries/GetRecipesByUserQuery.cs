using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Models.Read;

namespace fed.cloud.recipe.application.Query.Queries;

public class GetRecipesByUserQuery : IQuery<IEnumerable<RecipeModel>>
{
    public GetRecipesByUserQuery(Guid userId, int requestPageSize, int nextSkip)
    {
        UserId = userId;
        RequestPageSize = requestPageSize;
        NextSkip = nextSkip;
    }

    public Guid UserId { get; }

    public int RequestPageSize { get; }

    public int NextSkip { get; }
}

public class GetRecipesByUserQueryHandler : IQueryHandler<GetRecipesByUserQuery, IEnumerable<RecipeModel>>
{
    private readonly IFetchManager _fetchManager;

    public GetRecipesByUserQueryHandler(IFetchManager fetchManager)
    {
        _fetchManager = fetchManager;
    }

    public async Task<IEnumerable<RecipeModel>?> Handle(GetRecipesByUserQuery query)
    {
        var recipes = await _fetchManager.GetByPageAsync<RecipeModel>(query.RequestPageSize,
            query.NextSkip, r => r.CookingTime, true, r => r.OwnerId == query.UserId);
        foreach (var recipe in recipes)
        {
            recipe.Ingredients = await _fetchManager.GetAsync<RecipeIngredientModel>(ri => ri.RecipeId == recipe.Id);
        }
        
        return recipes;
    }
}