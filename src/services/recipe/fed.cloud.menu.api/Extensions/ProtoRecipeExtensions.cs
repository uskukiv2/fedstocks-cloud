using fed.cloud.menu.api.Protos;
using fed.cloud.menu.data.Entities;
using Google.Protobuf.Collections;

namespace fed.cloud.menu.api.Extensions;

public static class ProtoRecipeExtensions
{
    public static IEnumerable<RecipeIngredient> ToWrite(this IEnumerable<RecipeIngredientModel> ingredientDtos, Guid recipeId)
    {
        return ingredientDtos.Select(x => new RecipeIngredient()
        {
            RecipeId = recipeId,
            Quantity = x.Quantity,
            Ingredient = new Ingredient()
            {
                ProductNumber = x.ProductNumber,
                UnitId = x.UnitNumber
            }
        });
    }
}