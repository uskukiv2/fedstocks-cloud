using fed.cloud.store.domain.Abstract;
using fed.cloud.store.domain.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;

namespace fed.cloud.store.infrastructure.Factories.Implentations
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
            return (IUnitOfWork<TDbConnection>)_provider.GetRequiredService(typeof(IUnitOfWork<TDbConnection>));
        }
    }
}