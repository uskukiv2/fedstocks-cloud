using System;
using System.Data.Common;
using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Factories;
using Microsoft.Extensions.DependencyInjection;

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
            return (IUnitOfWork<TDbConnection>) _provider.GetRequiredService(typeof(IUnitOfWork<TDbConnection>));
        }
    }
}