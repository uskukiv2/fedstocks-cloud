#nullable enable
using System.Reactive;
using System.Windows.Input;
using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Application.Models.Recipes;
using gen.fedstocks.web.Client.Application.Services;
using PropertyChanged;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Application.ViewModels.Recipes;

[AddINotifyPropertyChangedInterface]
public class RecipeEditViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly IApplicationService _applicationService;
    private readonly int? _totalIngredientByDefault;
    private readonly int? _totalPreparationsByDefault;

    private ReactiveCommand<int, Unit>? _loadRecipeCommand;
    private ReactiveCommand<Unit, Unit>? _createNewRecipeCommand;
    private ReactiveCommand<Unit, int>? _saveRecipeChangesCommand;
    private ReactiveCommand<Unit, Unit>? _addIngredientToRecipeCommand;
    private ReactiveCommand<Unit, Unit>? _addPreparationToRecipeCommand;
    private ReactiveCommand<RecipeIngredientDto, Unit>? _removeIngredientFromRecipeCommand;
    private ReactiveCommand<RecipePreparationDto, Unit>? _removePreparationFromRecipeCommand;
    private ReactiveCommand<string, Unit>? _addTagCommand;
    private ReactiveCommand<string, Unit>? _removeTagCommand;

    public RecipeEditViewModel(IRecipeService recipeService, IApplicationService applicationService)
    {
        _recipeService = recipeService;
        _applicationService = applicationService;
        _totalIngredientByDefault = 2;
        _totalPreparationsByDefault = 2;
        IngredientUnitTypeValues = GetIngredientUnitTypeValues();
    }

    [AlsoNotifyFor(nameof(CurrentRecipe))]
    public RecipeDto CurrentRecipe { get; private set; }

    [AlsoNotifyFor(nameof(IngredientUnitTypeValues))]
    public IEnumerable<KeyValuePair<int, string>> IngredientUnitTypeValues { get; set; }

    [AlsoNotifyFor(nameof(IsPossibleToRemoveIngredient))]
    public bool IsPossibleToRemoveIngredient => CurrentRecipe.Ingredients.Count > _totalIngredientByDefault;

    [AlsoNotifyFor(nameof(IsPossibleToRemovePreparation))]
    public bool IsPossibleToRemovePreparation => CurrentRecipe.Preparations.Count > _totalPreparationsByDefault;

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

    public ReactiveCommand<RecipeIngredientDto, Unit> RemoveIngredientFromRecipeCommand =>
        _removeIngredientFromRecipeCommand ??= _removeIngredientFromRecipeCommand = ReactiveCommand.Create<RecipeIngredientDto>(RemoveIngredient);

    public ReactiveCommand<RecipePreparationDto, Unit> RemovePreparationFromRecipeCommand =>
        _removePreparationFromRecipeCommand ??= _removePreparationFromRecipeCommand = ReactiveCommand.Create<RecipePreparationDto>(RemovePreparation);

    public ReactiveCommand<string, Unit> AddTagCommand => _addTagCommand ??= _addTagCommand = ReactiveCommand.Create<string>(AddTag);

    public ReactiveCommand<string, Unit> RemoveTagCommand => _removeTagCommand ??= _removeTagCommand = ReactiveCommand.Create<string>(RemoveTag);

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
        for (var i = 0; i < _totalIngredientByDefault; i++)
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
        DoPropertyChanged(nameof(CurrentRecipe));
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

        DoPropertyChanged(nameof(CurrentRecipe));
    }

    private void RemoveIngredient(RecipeIngredientDto ingredient)
    {
        CurrentRecipe.Ingredients.Remove(ingredient);
        DoPropertyChanged(nameof(CurrentRecipe));
    }

    private void RemovePreparation(RecipePreparationDto preparation)
    {
        CurrentRecipe.Preparations.Remove(preparation);
        DoPropertyChanged(nameof(CurrentRecipe));
    }

    private void AddTag(string name)
    {
        if (string.IsNullOrEmpty(name) && CurrentRecipe.Tags.Any(x => x == name))
        {
            return;
        }
        
        CurrentRecipe.Tags.Add(name);
        DoPropertyChanged(nameof(CurrentRecipe));
    }

    private void RemoveTag(string tag)
    {
        CurrentRecipe.Tags.Remove(tag);
        DoPropertyChanged(nameof(CurrentRecipe));
    }
}