using System.Reactive;
using System.Windows.Input;
using gen.fedstocks.web.Client.Application.Abstract;
using gen.fedstocks.web.Client.Application.Services;
using PropertyChanged;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Application.ViewModels;

[AddINotifyPropertyChangedInterface]
public class NavigationViewModel : BaseViewModel
{
    private readonly IApplicationService _applicationService;

    private ReactiveCommand<Unit, Unit>? _reloadDataCommand;

    public NavigationViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
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