using gen.fedstocks.web.Client.Application.Abstract;

namespace gen.fedstocks.web.Client.Application.Factories;

public interface IViewModelFactory
{
    T Create<T>() where T : BaseViewModel;

    BaseViewModel Create(Type type);

}