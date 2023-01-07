using gen.fedstocks.web.server.Abstract;
using gen.fedstocks.web.server.Models;
using Microsoft.AspNetCore.Components;
using ReactiveUI;
using System.Collections.ObjectModel;
using gen.fed.application.Models.Recipes;
using gen.fed.application.ViewModels.Recipes;

namespace gen.fedstocks.web.server.Pages.Recipes;

[Route(RouterStrings.RecipeList)]
public partial class RecipeList : FedComponentBase<RecipeListViewModel>
{
    [Inject] public IMessageBus MessageBus { get; set; }

    public ObservableCollection<RecipeDto> Recipes => ViewModel.Recipes;

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
                Command = AddNewRecipeHandler
            }
        };
    }
}