using System.Data.Common;
using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Factories;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace fed.cloud.menu.infrastructure.Factories;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _provider;

    public UnitOfWorkFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IUnitOfWork<TDbConnection> Create<TDbConnection>() where TDbConnection : DbConnection
    {
        using var scope = _provider.GetService<IServiceScopeFactory>()?.CreateScope();
        return (IUnitOfWork<TDbConnection>)scope.ServiceProvider.GetRequiredService<MenuContext>()!;
    }

    public IUnitOfWork CreateDefault()
    {
        using var scope = _provider.GetService<IServiceScopeFactory>()!.CreateScope();
        return scope.ServiceProvider.GetRequiredService<MenuContext>();
    }
}