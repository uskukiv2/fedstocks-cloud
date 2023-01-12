using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Application.Factories;
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

    public RecipeListViewModel(NavigationState navigationState, IViewModelFactory viewModelFactory,
        IRecipeService recipeService, IApplicationService applicationService)
    {
        _navigationState = navigationState;
        _viewModelFactory = viewModelFactory;
        _recipeService = recipeService;
        _applicationService = applicationService;
        Recipes = new ObservableCollection<RecipeDto>();
    }

    [AlsoNotifyFor(nameof(Recipes))]
    public ObservableCollection<RecipeDto> Recipes { get; private set; }

    public ReactiveCommand<int, bool> IsRecipeEditPossibleCommand => _isRecipeEditPossibleCommand
        ??= _isRecipeEditPossibleCommand = ReactiveCommand.CreateFromTask<int, bool>(IsRecipeEditPossible);

    public ICommand RefreshRecipesCommand => _refreshRecipesCommand ??= _refreshRecipesCommand = ReactiveCommand.CreateFromTask(ReloadAsync);

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
        var recipes = await _recipeService.GetRecipesAsync(currentUser.UserId);

        Recipes = new ObservableCollection<RecipeDto>(recipes);
    }

    private async Task<bool> IsRecipeEditPossible(int id)
    {
        var currentUser = await _applicationService.GetCurrentUserAccountAsync();
        return await _recipeService.IsPossibleToEditRecipeAsync(currentUser.UserId, id);
    }
}