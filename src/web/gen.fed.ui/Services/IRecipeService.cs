﻿using gen.fed.ui.Abstract;
using gen.fed.ui.Models.Recipes;

namespace gen.fed.ui.Services;

public interface IRecipeService : IService
{
    Task<int> GetNextIngredientIdAsync(int id);
    Task<int> GetNextPreparationIdAsync(int id);
    Task<RecipeDto> GetRecipeAsync(int userId, int recipeId);
    Task<IEnumerable<RecipeDto>> GetRecipesAsync(int userId);
    Task<bool> IsPossibleToEditRecipeAsync(int userId, int id);
    Task<int> SaveChangesAsync(RecipeDto currentRecipe, int userId);

    bool IsExist(int id);
}