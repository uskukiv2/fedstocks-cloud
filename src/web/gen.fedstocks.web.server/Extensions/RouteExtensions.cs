using gen.fed.application.ViewModels.Recipes;
using gen.fedstocks.web.server.Models;

namespace gen.fedstocks.web.server.Extensions;

public static class RouteExtensions
{
    public static IEnumerable<KeyValuePair<string, Type>> GetRoutes()
    {
        return new[]
        {
            new KeyValuePair<string, Type>(RouterStrings.RecipeList, typeof(RecipeListViewModel)),
            new KeyValuePair<string, Type>(RouterStrings.RecipeNew, typeof(RecipeEditViewModel)),
            new KeyValuePair<string, Type>(RouterStrings.RecipeEdit, typeof(RecipeEditViewModel))
        };
    }
}