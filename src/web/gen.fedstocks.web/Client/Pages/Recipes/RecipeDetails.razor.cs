using System.Reactive;
using gen.fedstocks.web.Client.Abstract;
using gen.fedstocks.web.Client.Application.Models.Products;
using gen.fedstocks.web.Client.Application.Models.Recipes;
using gen.fedstocks.web.Client.Application.ViewModels.Recipes;
using gen.fedstocks.web.Client.Models;
using gen.fedstocks.web.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using PropertyChanged;

namespace gen.fedstocks.web.Client.Pages.Recipes
{
    [Route(RouterStrings.Recipe)]
    [Route(RouterStrings.RecipeNew)]
    public partial class RecipeDetails : FedComponentBase<RecipeEditViewModel>, IEditable
    {
        [Parameter] public int RecipeId { get; set; }

        [Inject] public IDialogService DialogService { get; set; }

        [AlsoNotifyFor(nameof(CurrentRecipe))] public RecipeDto CurrentRecipe { get; set; } = new();

        [AlsoNotifyFor(nameof(EditContext))] public EditContext EditContext { get; private set; }

        [AlsoNotifyFor(nameof(TotalRecipeContents))]
        public int TotalRecipeContents => CurrentRecipe.Preparations.Count;

        [AlsoNotifyFor(nameof(IsEditMode))] public bool IsEditMode { get; set; }

        public IEnumerable<UnitDto> IngredientUnitTypes { get; private set; }

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

        private async Task AddTag()
        {
            var result = await DialogService.Show<DialogInputComponent>("Type tag name", new DialogOptions()
            {
                Position = DialogPosition.Center,
                CloseButton = false,
                CloseOnEscapeKey = true
            }).Result;
            
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

            ViewModel.GetUnitsCommand.Execute(Unit.Default).Subscribe(x =>
            {
                IngredientUnitTypes = x;
            });

            return base.OnAfterRenderAsync(isFirstRender);
        }

        protected override void OnParametersSet()
        {
            EditContext = new EditContext(CurrentRecipe);
            base.OnParametersSet();
        }
    }
}