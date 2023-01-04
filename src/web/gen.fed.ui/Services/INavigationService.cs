using gen.fed.ui.Abstract;

namespace gen.fed.ui.Services;

public interface ICurrentViewModelObservableContainer
{
    public IObservable<BaseViewModel?> CurrentViewModel { get; }
}