using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace fed.cloud.menu.infrastructure.Factories;

public interface IRepositoryFactory
{
    T Create<T>() where T : IRepository;
}

public class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public T Create<T>() where T : IRepository
    {
         using var scope = _serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope();
         return scope.ServiceProvider.GetRequiredService<T>();
    }
}