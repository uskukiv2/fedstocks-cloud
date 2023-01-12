using gen.fedstocks.web.Client.Abstract;
using gen.fedstocks.web.Client.Application.ViewModels.Recipes;
using gen.fedstocks.web.Client.Models;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Pages.Recipes;

[Route(RouterStrings.RecipeList)]
public partial class RecipeList : FedComponentBase<RecipeListViewModel>
{
    [Inject] public IMessageBus MessageBus { get; set; }

    private void AddNewRecipeHandler()
    {
        Navigation.NavigateTo(RouterStrings.RecipeNew);
    }

    private void ViewSelectedRecipe(int id)
    {
        Navigation.NavigateTo(string.Format(RouterStrings.RecipeParsable, id));
    }

    protected override IEnumerable<(string, Action)> GetMenuItems()
    {
        return new (string Item, Action Command)[]
        {
            new()
            {
                Item = CommandNames.AddCommandName,
                Command = AddNewRecipeHandler,
            }
        };
    }
}