using fed.cloud.menu.api.Extensions;
using fed.cloud.menu.data.Entities;
using Mapster;

namespace fed.cloud.menu.api.Mappings;

public class RecipeRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Protos.RecipeModel, Recipe>()
            .Map(x => x.Id, y => Guid.Parse(y.Id))
            .Map(x => x.Name, y => y.Name)
            .Map(x => x.Tags, y => y.Tags)
            .Map(x => x.CookingTime, y => y.CookingTime.ToTimeSpan())
            .Map(x => x.Content, y => y.Content)
            .Map(x => x.Ingredients, y => y.RecipeIngredients.ToWrite(Guid.Parse(y.Id)));
    }
}