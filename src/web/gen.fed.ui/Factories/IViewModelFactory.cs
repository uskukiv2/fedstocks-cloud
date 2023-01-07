using gen.fed.application.Abstract;

namespace gen.fed.application.Factories;

public interface IViewModelFactory
{
    T Create<T>() where T : BaseViewModel;

    BaseViewModel Create(Type type);

}