using System.Collections.ObjectModel;
using fed.cloud.communication.Recipe;
using gen.fedstocks.web.Client.Application.Models.Recipes;

namespace gen.fedstocks.web.Client.Application.Extensions;

public static class RecipeExtensions
{
    public static IEnumerable<RecipeDto> ToDto(this IEnumerable<Recipe>? recipes)
    {
        if (recipes == null) throw new ArgumentNullException(nameof(recipes));
        
        var recipeArray = recipes.ToArray();
        for (var i = 0; i < recipeArray.Length; i++)
        {
            yield return new RecipeDto()
            {
                Id = i + 1,
                RecipeId = recipeArray[i].Id,
                Name = recipeArray[i].RecipeName,
                Tags = CreateTags(recipeArray[i].Tags),
                Preparations = CreatePreparations(recipeArray[i].Contents),
                Ingredients = CreateIngredients(recipeArray[i].Ingredients)
            };
        }
    }

    private static ObservableCollection<RecipeIngredientDto> CreateIngredients(IEnumerable<RecipeIngredient> ingredients)
    {
        var ingredientArray = ingredients.ToArray();
        var ingredientList = new List<RecipeIngredientDto>();

        for (var i = 0; i < ingredientArray.Length; i++)
        {
            var id = i + 1;
            ingredientList.Add(new RecipeIngredientDto(id, ingredientArray[i].Id)
            {
                Name = ingredientArray[i].IngredientName,
                Quantity = ingredientArray[i].Quantity
            });
        }

        return new ObservableCollection<RecipeIngredientDto>(ingredientList.OrderBy(x => x.Id));
    }

    private static ObservableCollection<RecipePreparationDto> CreatePreparations(IEnumerable<string> contents)
    {
        var contentArray = contents.ToArray();
        var preparationList = new List<RecipePreparationDto>();

        for (var i = 0; i < contentArray.Length; i++)
        {
            var id = i + 1;
            preparationList.Add(new RecipePreparationDto()
            {
                Id = id,
                Content = contentArray[i]
            });
        }

        return new ObservableCollection<RecipePreparationDto>(preparationList.OrderBy(x => x.Id));
    }

    private static ObservableCollection<string> CreateTags(IEnumerable<string> tags)
    {
        return new ObservableCollection<string>(tags);
    }
}