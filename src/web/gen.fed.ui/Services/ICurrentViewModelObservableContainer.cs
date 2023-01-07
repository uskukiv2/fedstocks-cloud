using gen.fed.application.Abstract;

namespace gen.fed.application.Services;

public interface ICurrentViewModelObservableContainer
{
    public IObservable<BaseViewModel?> CurrentViewModel { get; }
}