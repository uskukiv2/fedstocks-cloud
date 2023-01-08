using System.Reactive;
using System.Windows.Input;
using gen.fed.application.Abstract;
using gen.fed.application.Services;
using PropertyChanged;
using ReactiveUI;

namespace gen.fed.application.ViewModels;

[AddINotifyPropertyChangedInterface]
public class NavigationViewModel : BaseViewModel
{
    private readonly IApplicationService _applicationService;
    private readonly IUserService _userService;

    private ReactiveCommand<Unit, Unit>? _reloadDataCommand;

    public NavigationViewModel(IApplicationService applicationService, IUserService userService)
    {
        _applicationService = applicationService;
        _userService = userService;
    }

    [AlsoNotifyFor(nameof(IsUserHasIntegrations))]
    public bool IsUserHasIntegrations { get; private set; }

    [AlsoNotifyFor(nameof(IsUserAdmin))]
    public bool IsUserAdmin { get; private set; }

    public ICommand ReloadDataCommand => _reloadDataCommand ??=
        _reloadDataCommand = ReactiveCommand.CreateFromTask(ReloadData);

    private async Task ReloadData()
    {
        IsUserHasIntegrations = false;
        IsUserAdmin = false;
    }
}