using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using gen.fed.application.Abstract;
using ReactiveUI;

namespace gen.fed.application.Services.Implementation;

public class NavigationState : ICurrentViewModelObservableContainer
{
    public NavigationState()
    {
        NavigationStack = new ObservableCollection<BaseViewModel>();
        Create();
    }

    public ObservableCollection<BaseViewModel> NavigationStack { get; }

    public IObservable<IChangeSet<BaseViewModel>> NavigationChanged { get; protected set; }

    public IObservable<BaseViewModel?> CurrentViewModel { get; protected set; }

    internal ReactiveCommand<BaseViewModel, BaseViewModel> NavigateAndReset { get; private set; }

    internal ReactiveCommand<BaseViewModel, BaseViewModel> Navigate { get; private set; }

    private void Create()
    {
        NavigationChanged = NavigationStack.ToObservableChangeSet();

        Navigate = ReactiveCommand.CreateFromObservable<BaseViewModel, BaseViewModel>(vm =>
        {
            NavigationStack.Add(vm);

            return Observable.Return(vm);
        });

        NavigateAndReset = ReactiveCommand.CreateFromObservable<BaseViewModel, BaseViewModel>(vm =>
        {
            NavigationStack.Clear();

            return Navigate.Execute(vm);
        });

        CurrentViewModel = Observable.Defer(() => Observable.Return(NavigationStack.LastOrDefault()))
            .Concat(NavigationChanged.Select(_ => NavigationStack.LastOrDefault())).Where(x => x != null);
    }
}