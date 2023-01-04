namespace gen.fedstocks.web.server.Models;

public static class RouterStrings
{
    public const string RecipeList = "recipe/list";
    public const string RecipeNew = "recipe/new";
    
    /// <summary>
    /// Used only to define route for EditingPage with parameter
    /// </summary>
    /// <remarks>Use <see cref="RecipeEditParsable"/> for navigating between pages instead</remarks>
    public const string RecipeEdit = "recipe/edit/{RecipeId:int}";
    public const string RecipeEditParsable = "recipe/edit/{0}";

    /// <summary>
    /// Used only to define route for ViewPage with parameter
    /// </summary>
    /// <remarks>Use <see cref="RecipeViewParsable"/> for navigating between pages instead</remarks>
    public const string Recipe = "recipe/{RecipeId:int}";
    public const string RecipeParsable = "recipe/{0}";
}