using gen.fed.ui.Models.Recipes;
using System.Security.Cryptography.X509Certificates;

namespace gen.fed.ui.Services.Implementation;

public class RecipeService : IRecipeService
{
    private IList<RecipeDto> _recipes;

    public RecipeService()
    {
        _recipes = new List<RecipeDto>();
    }

    public async Task<RecipeDto> GetRecipeAsync(int userId, int recipeId)
    {
        return _recipes.FirstOrDefault(x => x.Id == recipeId)!;
    }

    public async Task<IEnumerable<RecipeDto>> GetRecipesAsync(int userId)
    {
        return _recipes;
    }

    public async Task<bool> IsPossibleToEditRecipeAsync(int userId, int id)
    {
        return true;
    }

    public async Task<int> SaveChangesAsync(RecipeDto recipe, int userId)
    {
        ValidateNumbersOfPreparationsOrdering(recipe.Preparations);
        if (recipe.Id < 1)
        {
            recipe.Id = _recipes.Count + 1;
            _recipes.Add(recipe);
            return recipe.Id;
        }

        UpdateRecipe(recipe);
        return recipe.Id;
    }

    public Task<int> GetNextIngredientIdAsync(int id)
    {
        return Task.FromResult(_recipes.SingleOrDefault(x => x.Id == id)!.Ingredients.Max(y => y.Id));
    }

    public Task<int> GetNextPreparationIdAsync(int id)
    {
        return Task.FromResult(_recipes.SingleOrDefault(x => x.Id == id)!.Preparations.Max(y => y.Id));
    }

    public bool IsExist(int id)
    {
        return _recipes.Any(x => x.Id == id);
    }

    private void UpdateRecipe(RecipeDto recipe)
    {
        var currentRecipe = _recipes.FirstOrDefault(x => x.Id == recipe.Id);
        _recipes.Remove(currentRecipe);
        _recipes.Add(recipe);
    }

    private void ValidateNumbersOfPreparationsOrdering(IEnumerable<RecipePreparationDto> preparationDtos)
    {

    }
}