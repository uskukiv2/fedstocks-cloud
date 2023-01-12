using gen.fedstocks.web.Client.Application.Abstract;

namespace gen.fedstocks.web.Client.Application.Services;

public interface ICurrentViewModelObservableContainer
{
    public IObservable<BaseViewModel?> CurrentViewModel { get; }
}