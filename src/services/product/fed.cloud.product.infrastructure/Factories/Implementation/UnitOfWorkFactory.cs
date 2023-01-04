using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;

namespace fed.cloud.product.infrastructure.Factories.Implementation
{
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
            return (IUnitOfWork<TDbConnection>)scope.ServiceProvider.GetRequiredService<ProductContext>();
        }
    }
}