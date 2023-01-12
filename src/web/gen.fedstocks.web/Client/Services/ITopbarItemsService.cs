using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using DynamicData;
using gen.fedstocks.web.Client.Abstract;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Services;

public interface ITopbarItemsService : IFedService, INotifyPropertyChanged, IDisposable
{
    ObservableCollection<string> RightMenuItems { get; }

    ReactiveCommand<string, Unit> MenuItemClickedCommand { get; }

    IObservable<string> AddMenuItems(IEnumerable<string> items);
}

public class TopbarItemsService : ITopbarItemsService
{
    private readonly NavigationManager _navigation;

    private ObservableCollection<string> _rightMenuItems;
    private ReactiveCommand<string, Unit>? _menuItemClickedCommand;
    private ISubject<string> _currentSubject;

    public TopbarItemsService(NavigationManager navigation)
    {
        _navigation = navigation;
        _rightMenuItems = new ObservableCollection<string>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> RightMenuItems
    {
        get => _rightMenuItems;
        private init
        {
            _rightMenuItems = value;
            OnPropertyChanged();
        }
    }

    public ReactiveCommand<string, Unit> MenuItemClickedCommand =>
        _menuItemClickedCommand ??= _menuItemClickedCommand = ReactiveCommand.Create<string>(MenuItemClicked);

    public IObservable<string> AddMenuItems(IEnumerable<string> items)
    {
        if (RightMenuItems.Any())
        {
            RightMenuItems.Clear();
        }
        RightMenuItems.AddRange(items);

        _currentSubject = new ScheduledSubject<string>(CurrentThreadScheduler.Instance, null, new BehaviorSubject<string>(default!));

        return _currentSubject;
    }

    public void Dispose()
    {
        _currentSubject?.OnCompleted();
        _currentSubject = null!;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void MenuItemClicked(string commandName)
    {
        _currentSubject.OnNext(commandName);
    }
}