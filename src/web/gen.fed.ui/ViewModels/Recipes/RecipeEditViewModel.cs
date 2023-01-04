#nullable enable
using gen.fed.ui.Abstract;
using gen.fed.ui.Models.Recipes;
using gen.fed.ui.Services;
using PropertyChanged;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace gen.fed.ui.ViewModels.Recipes;

[AddINotifyPropertyChangedInterface]
public class RecipeEditViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly IApplicationService _applicationService;
    private readonly int? _ingredientCountByDefault;

    private ReactiveCommand<int, Unit>? _loadRecipeCommand;
    private ReactiveCommand<Unit, Unit>? _createNewRecipeCommand;
    private ReactiveCommand<Unit, int>? _saveRecipeChangesCommand;
    private ReactiveCommand<Unit, Unit>? _addIngredientToRecipeCommand;
    private ReactiveCommand<Unit, Unit>? _addPreparationToRecipeCommand;
    private ReactiveCommand<int, Unit>? _removeIngredientFromRecipeCommand;
    private ReactiveCommand<int, Unit>? _removePreparationFromRecipeCommand;

    public RecipeEditViewModel(IRecipeService recipeService, IApplicationService applicationService)
    {
        _recipeService = recipeService;
        _applicationService = applicationService;
        _ingredientCountByDefault = 2;
        IngredientUnitTypeValues = GetIngredientUnitTypeValues();
    }

    [AlsoNotifyFor(nameof(CurrentRecipe))]
    public RecipeDto CurrentRecipe { get; private set; }

    [AlsoNotifyFor(nameof(IngredientUnitTypeValues))]
    public IEnumerable<KeyValuePair<int, string>> IngredientUnitTypeValues { get; set; }

    [AlsoNotifyFor(nameof(IsPossibleToRemoveIngredient))]
    public bool IsPossibleToRemoveIngredient => CurrentRecipe.Ingredients.Count > _ingredientCountByDefault;

    [AlsoNotifyFor(nameof(IsPossibleToEditRecipe))]
    public bool IsPossibleToEditRecipe { get; private set; }

    public ICommand LoadRecipeCommand =>
        _loadRecipeCommand ??= _loadRecipeCommand = ReactiveCommand.CreateFromTask<int>(LoadRecipe);

    public ICommand CreateNewRecipeCommand =>
        _createNewRecipeCommand ??= _createNewRecipeCommand = ReactiveCommand.CreateFromTask(CreateNewRecipe);

    public ReactiveCommand<Unit, int> SaveRecipeChangesCommand => _saveRecipeChangesCommand ??=
        _saveRecipeChangesCommand = ReactiveCommand.CreateFromTask(SaveRecipeChanges);

    public ICommand AddIngredientToRecipeCommand =>
        _addIngredientToRecipeCommand ??= _addIngredientToRecipeCommand = ReactiveCommand.CreateFromTask(AddIngredient);

    public ICommand AddPreparationToRecipeCommand =>
        _addPreparationToRecipeCommand ??= _addPreparationToRecipeCommand = ReactiveCommand.CreateFromTask(AddPreparation);

    public ReactiveCommand<int, Unit> RemoveIngredientFromRecipeCommand =>
        _removeIngredientFromRecipeCommand ??= _removeIngredientFromRecipeCommand = ReactiveCommand.Create<int>(RemoveIngredient);

    public ReactiveCommand<int, Unit> RemovePreparationFromRecipeCommand =>
        _removePreparationFromRecipeCommand ??= _removePreparationFromRecipeCommand = ReactiveCommand.Create<int>(RemovePreparation);

    private IEnumerable<KeyValuePair<int, string>> GetIngredientUnitTypeValues()
    {
        return from int value in Enum.GetValues(typeof(IngredientUnitType))
               select new KeyValuePair<int, string>(value, Enum.GetName(typeof(IngredientUnitType), value)!);
    }

    private async Task CreateNewRecipe()
    {
        CurrentRecipe = new RecipeDto();
        await AddDefaultIngredients();
    }

    private async Task<int> SaveRecipeChanges()
    {
        var currentUser = await _applicationService.GetCurrentUserAccountAsync();
        return await _recipeService.SaveChangesAsync(CurrentRecipe, currentUser.UserId).ConfigureAwait(false);
    }

    private async Task LoadRecipe(int recipeId)
    {
        var currentUser = await _applicationService.GetCurrentUserAccountAsync();

        CurrentRecipe = await _recipeService.GetRecipeAsync(currentUser.UserId, recipeId);
        CurrentRecipe.Preparations.OrderBy(x => x.NumberOfOrder);
        IsPossibleToEditRecipe = await _recipeService.IsPossibleToEditRecipeAsync(currentUser.UserId, CurrentRecipe.Id);
    }

    private async Task AddDefaultIngredients()
    {
        for (var i = 0; i < _ingredientCountByDefault; i++)
        {
            await AddIngredient();
        }
    }

    private async Task AddIngredient()
    {
        int nextIngredientId;
        if (_recipeService.IsExist(CurrentRecipe.Id))
        {
            nextIngredientId = await _recipeService.GetNextIngredientIdAsync(CurrentRecipe.Id);
        }
        else
        {
            nextIngredientId = CurrentRecipe.Ingredients.Count + 1;
        }

        CurrentRecipe.Ingredients.Add(new RecipeIngredientDto(nextIngredientId));
    }

    private async Task AddPreparation()
    {
        int nextPreparationId;
        if (_recipeService.IsExist(CurrentRecipe.Id))
        {
            nextPreparationId = await _recipeService.GetNextPreparationIdAsync(CurrentRecipe.Id);
        }
        else
        {
            nextPreparationId = CurrentRecipe.Ingredients.Count + 1;
        }
        CurrentRecipe.Preparations.Add(new RecipePreparationDto(nextPreparationId)
        {
            NumberOfOrder = CurrentRecipe.Preparations.Count + 1,
        });
    }

    private void RemoveIngredient(int id)
    {
        CurrentRecipe.Ingredients.Remove(CurrentRecipe.Ingredients.SingleOrDefault(x => x.Id == id)!);
    }

    private void RemovePreparation(int id)
    {
        CurrentRecipe.Preparations.Remove(CurrentRecipe.Preparations.SingleOrDefault(x => x.Id == id)!);
    }
}