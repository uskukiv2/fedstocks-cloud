using gen.fed.ui.Abstract;

namespace gen.fed.ui.Factories;

public interface IViewModelFactory
{
    T Create<T>() where T : BaseViewModel;

    BaseViewModel Create(Type type);

}