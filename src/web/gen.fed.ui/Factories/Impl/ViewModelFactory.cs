using gen.fed.application.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace gen.fed.application.Factories.Impl;

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _provider;

    public ViewModelFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public T Create<T>() where T : BaseViewModel
    {
        return (T)Create(typeof(T));
    }

    public BaseViewModel Create(Type type)
    {
        return (BaseViewModel)CreateByType(type);
    }

    private object CreateByType(Type type)
    {
        return _provider.GetRequiredService(type);
    }

}