using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Application.Factories;
using gen.fedstocks.web.Client.Application.Models;
using gen.fedstocks.web.Client.Application.Models.Recipes;
using gen.fedstocks.web.Client.Application.Services;
using gen.fedstocks.web.Client.Application.Services.Implementation;
using PropertyChanged;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Application.ViewModels.Recipes;

[AddINotifyPropertyChangedInterface]
public class RecipeListViewModel : BaseViewModel
{
    private readonly NavigationState _navigationState;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IRecipeService _recipeService;
    private readonly IApplicationService _applicationService;

    private ReactiveCommand<int, bool>? _isRecipeEditPossibleCommand;
    private ReactiveCommand<Unit, Unit>? _refreshRecipesCommand;

    private int _previousRecipesLoaded;
    private int _currentRecipesLoaded;

    public RecipeListViewModel(NavigationState navigationState, IViewModelFactory viewModelFactory,
        IRecipeService recipeService, IApplicationService applicationService)
    {
        _navigationState = navigationState;
        _viewModelFactory = viewModelFactory;
        _recipeService = recipeService;
        _applicationService = applicationService;
        _previousRecipesLoaded = 0;
        _currentRecipesLoaded = 0;
        Recipes = new ObservableCollection<RecipeDto>();
    }

    [AlsoNotifyFor(nameof(Recipes))]
    public ObservableCollection<RecipeDto> Recipes { get; private set; }

    public ReactiveCommand<int, bool> IsRecipeEditPossibleCommand => _isRecipeEditPossibleCommand
        ??= _isRecipeEditPossibleCommand = ReactiveCommand.CreateFromTask<int, bool>(IsPossibleToEditRecipe);

    public ICommand RefreshRecipesCommand => _refreshRecipesCommand ??=
        _refreshRecipesCommand = ReactiveCommand.CreateFromTask(ReloadAsync);

    [AlsoNotifyFor(nameof(IsPossibleToGoForward))]
    public bool IsPossibleToGoForward => _currentRecipesLoaded == AppConstValues.MaxRecipePageSize;

    public bool IsPossibleToGoBackward => _previousRecipesLoaded > 0;
    
    public override async Task InitAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        await ReloadAsync();

        IsInitialized = true;
    }

    private async Task ReloadAsync()
    {
        var currentUser = await _applicationService.GetCurrentUserAccountAsync();
        await LoadRecipesAsync(currentUser.UserId, AppConstValues.MaxRecipePageSize, 0);
        _currentRecipesLoaded += Recipes.Count;
    }

    private async Task LoadNextAsync()
    {
        var currentUser = await _applicationService.GetCurrentUserAccountAsync();
        await LoadRecipesAsync(currentUser.UserId, AppConstValues.MaxRecipePageSize,
            Recipes.Count + AppConstValues.MaxRecipePageSize);
        _currentRecipesLoaded += Recipes.Count;
    }

    private async Task LoadPreviousAsync()
    {
        var currentUser = await _applicationService.GetCurrentUserAccountAsync();
        await LoadRecipesAsync(currentUser.UserId, AppConstValues.MaxRecipePageSize,
            _currentRecipesLoaded - AppConstValues.MaxRecipePageSize);
        _currentRecipesLoaded -= Recipes.Count;
    }

    private async Task LoadRecipesAsync(int userId, int take, int skip)
    {
        var recipes = await _recipeService.GetRecipesAsync(userId, take, skip);

        Recipes = new ObservableCollection<RecipeDto>(recipes);
    }

    private async Task<bool> IsPossibleToEditRecipe(int id)
    {
        return true;
    }
}