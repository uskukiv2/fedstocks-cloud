using System.Security.Cryptography;
using fed.cloud.communication.Recipe;
using gen.fedstocks.web.Client.Application.Models.Recipes;
using Mapster;

namespace gen.fedstocks.web.Client.Application.Mappings;

public class RecipeRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RecipeDto, Recipe>()
            .Map(x => x.Id, y => y.RecipeId)
            .Map(x => x.RecipeName, y => y.Name)
            .Map(x => x.Tags, y => y.Tags)
            .Map(x => x.CookingTime, y => y.CookingTime)
            .Map(x => x.Ingredients, y => y.Ingredients.Select(i => new RecipeIngredient()
            {
                Id = i.RecipeIngredientId,
                IngredientName = i.Name,
                Quantity = i.Quantity,
                ReferenceNumber = 0,
                Unit = 0
            }))
            .Map(x => x.Contents, y => y.Preparations
                .OrderBy(x => x.Id)
                .Select(p => p.Content));
    }
}