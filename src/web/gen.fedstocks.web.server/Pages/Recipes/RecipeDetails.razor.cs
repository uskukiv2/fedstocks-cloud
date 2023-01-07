using gen.fed.application.Models.Recipes;
using gen.fed.application.ViewModels.Recipes;
using gen.fedstocks.web.server.Abstract;
using gen.fedstocks.web.server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PropertyChanged;

namespace gen.fedstocks.web.server.Pages.Recipes
{
    [Route(RouterStrings.Recipe)]
    [Route(RouterStrings.RecipeNew)]
    public partial class RecipeDetails : FedComponentBase<RecipeEditViewModel>, IEditable
    {
        [Parameter] public int RecipeId { get; set; } = 0;

        [AlsoNotifyFor(nameof(CurrentRecipe))]
        public RecipeDto CurrentRecipe { get; set; } = new RecipeDto();

        public EditContext EditContext { get; private set; }

        public int TotalRecipeContents { get; private set; } = 0;

        public int TotalTags { get; private set; } = 4;

        public bool IsEditMode { get; set; }

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
            UpdateMenu();
            StateHasChanged();
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
                TotalRecipeContents = ViewModel.CurrentRecipe.Preparations.Count;
            }

            CurrentRecipe = ViewModel.CurrentRecipe;
        }

        private void OnMarkdownValueChanged(int id, string changedValue)
        {
            if (string.IsNullOrEmpty(changedValue))
            {
                return;
            }

            if (CurrentRecipe.Preparations.Any(x => x.Id == id))
            {
                CurrentRecipe.Preparations.FirstOrDefault(x => x.Id == id)!.Content = changedValue;
            }
        }

        private void OnAddIngredientClicked()
        {
            ViewModel.AddIngredientToRecipeCommand.Execute(null);
        }

        private void OnAddPreparationClicked()
        {
            ViewModel.AddPreparationToRecipeCommand.Execute(null);
        }

        protected override IEnumerable<(string, Action)> GetMenuItems()
        {
            return !IsEditMode ? GetMenuItemsForViewMode() : GetMenuItemsForEditMode();
        }

        protected override Task OnAfterRenderAsync(bool isFirstRender)
        {
            if (!isFirstRender) return Task.CompletedTask;

            Init();

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
