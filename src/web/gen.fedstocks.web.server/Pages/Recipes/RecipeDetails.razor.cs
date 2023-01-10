using gen.fed.application.Models.Recipes;
using gen.fed.application.ViewModels.Recipes;
using gen.fedstocks.web.server.Abstract;
using gen.fedstocks.web.server.Models;
using gen.fedstocks.web.server.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using Newtonsoft.Json.Converters;
using PropertyChanged;

namespace gen.fedstocks.web.server.Pages.Recipes
{
    [Route(RouterStrings.Recipe)]
    [Route(RouterStrings.RecipeNew)]
    public partial class RecipeDetails : FedComponentBase<RecipeEditViewModel>, IEditable
    {
        private Random _random;
        
        [Parameter] public int RecipeId { get; set; }

        [Inject] public IDialogService DialogService { get; set; }

        [AlsoNotifyFor(nameof(CurrentRecipe))] public RecipeDto CurrentRecipe { get; set; } = new();

        [AlsoNotifyFor(nameof(EditContext))] public EditContext EditContext { get; private set; }

        [AlsoNotifyFor(nameof(TotalRecipeContents))]
        public int TotalRecipeContents => CurrentRecipe.Preparations.Count;

        [AlsoNotifyFor(nameof(IsEditMode))] public bool IsEditMode { get; set; }

        public IEnumerable<IngredientUnitType> IngredientUnitTypes { get; private set; }

        private void OnEditStartedCommand()
        {
            IsEditMode = true;
            EditContext = new EditContext(CurrentRecipe);
            UpdateMenu();
            StateHasChanged();
        }

        private void OnSaveCommand()
        {
            if (!EditContext.Validate())
            {
                return;
            }

            ViewModel.SaveRecipeChangesCommand.Execute().Subscribe(x =>
            {
                RecipeId = x;
                Init(true);
                UpdateMenu();
                StateHasChanged();
            });
        }

        private void OnCancelCommand()
        {
            IsEditMode = false;
            if (RecipeId > 0)
            {
                UpdateMenu();
                Init(true);
                StateHasChanged();
            }
            else
            {
                Navigation.NavigateTo(RouterStrings.RecipeList);
            }
        }

        private IEnumerable<(string, Action)> GetMenuItemsForViewMode()
        {
            return new (string name, Action command)[]
            {
                new(CommandNames.EditCommandName, OnEditStartedCommand),
            };
        }

        private IEnumerable<(string, Action)> GetMenuItemsForEditMode()
        {
            return new (string name, Action command)[]
            {
                new(CommandNames.SaveCommandName, OnSaveCommand),
                new(CommandNames.CancelCommandName, OnCancelCommand)
            };
        }

        private void Init(bool isReload = false)
        {
            if (RecipeId < 1 && !isReload)
            {
                ViewModel.CreateNewRecipeCommand.Execute(null);
                IsEditMode = true;
            }
            else
            {
                IsEditMode = false;
                ViewModel.LoadRecipeCommand.Execute(RecipeId);
            }

            CurrentRecipe = ViewModel.CurrentRecipe;
        }

        private void OnAddIngredientClicked()
        {
            ViewModel.AddIngredientToRecipeCommand.Execute(null);
        }

        private void OnAddPreparationClicked()
        {
            ViewModel.AddPreparationToRecipeCommand.Execute(null);
        }

        private async Task AddTag()
        {
            var result = await (await DialogService.ShowAsync<DialogInputComponent>("Type tag name", new DialogOptions()
            {
                Position = DialogPosition.Center,
                CloseButton = false,
                CloseOnEscapeKey = true
            })).Result;
            
            if (result.Cancelled)
            {
                return;
            }

            ViewModel.AddTagCommand.Execute((string)result.Data).Subscribe();
        }

        protected override IEnumerable<(string, Action)> GetMenuItems()
        {
            return !IsEditMode ? GetMenuItemsForViewMode() : GetMenuItemsForEditMode();
        }

        protected override Task OnAfterRenderAsync(bool isFirstRender)
        {
            if (!isFirstRender) return Task.CompletedTask;

            Init();

            _random = new Random(RecipeId);
            
            IngredientUnitTypes = Enum.GetValues<IngredientUnitType>();

            return base.OnAfterRenderAsync(isFirstRender);
        }

        protected override void OnParametersSet()
        {
            EditContext = new EditContext(CurrentRecipe);
            base.OnParametersSet();
        }
    }
}