using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Application.Models.Recipes;

namespace gen.fedstocks.web.Client.Application.Services;

public interface IRecipeService : IService
{
    Task<RecipeDto?> GetRecipeAsync(int userId, int recipeId);
    Task<IEnumerable<RecipeDto>> GetRecipesAsync(int userId, int recipePageSize, int skipRecipe);
    Task<int> SaveChangesAsync(RecipeDto currentRecipe, int userId);
}