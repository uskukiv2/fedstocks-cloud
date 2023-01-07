using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System;
using gen.fed.web.domain.Abstract;
using gen.fed.web.domain.Factories;

namespace gen.fed.web.infrastructure.Factories;

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
        return (IUnitOfWork<TDbConnection>)scope.ServiceProvider.GetRequiredService<ServiceContext>();
    }
}