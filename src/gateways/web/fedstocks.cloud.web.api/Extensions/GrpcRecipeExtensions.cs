using fed.cloud.menu.api.Protos;

namespace fedstocks.cloud.web.api.Extensions;

public static class GrpcRecipeExtensions
{
    public static IEnumerable<string> UnpackContent(this RecipeModel recipeModel)
    {
        return recipeModel.Content.Split(';').Select(x => x.Replace("{{", string.Empty))
            .Select(x => x.Replace("}}", string.Empty));
    }
}