using fed.cloud.communication.Recipe;
using fedstocks.cloud.web.api.Extensions;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Mapster;

using Proto = fed.cloud.menu.api.Protos;

namespace fedstocks.cloud.web.api.Mappings;

public class RecipeRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Proto.RecipeModel, Recipe>()
            .Map(x => x.RecipeName, y => y.Name)
            .Map(x => x.Id, y => y.Id.Adapt<Guid>())
            .Map(x => x.Tags, y => y.Tags.Split(';', StringSplitOptions.None))
            .Map(x => x.CookingTime, y => y.CookingTime.ToTimeSpan())
            .Map(x => x.Contents, y => y.Content.Split(';', StringSplitOptions.None))
            .Map(x => x.Ingredients, y => y.UnpackContent());
    }
}