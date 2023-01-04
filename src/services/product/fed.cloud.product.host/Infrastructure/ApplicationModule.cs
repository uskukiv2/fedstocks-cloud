using Autofac;
using fed.cloud.eventbus.Base;
using fed.cloud.product.application.IntegrationEvents.Events;
using fed.cloud.product.application.IntegrationEvents.Handlers;
using fed.cloud.product.application.Queries;
using fed.cloud.product.application.Queries.Implementation;
using fed.cloud.product.domain.Repository;
using fed.cloud.product.infrastructure.Repositories;
using System.Reflection;

namespace fed.cloud.product.host.Infrastructure;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ProductQuery>()
            .As<IProductQuery>()
            .InstancePerLifetimeScope();

        builder.RegisterType<SellerQuery>()
            .As<ISellerQuery>()
            .InstancePerLifetimeScope();

        builder.RegisterType<CountryQuery>()
            .As<ICountryQuery>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ProductRepository>()
            .As<IProductRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<SellerCompanyRepository>()
            .As<ISellerCompanyRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<CountryRepository>()
            .As<ICountryRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(AddProductPurchasesEvent).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        builder.RegisterType<AddProductPurchasesEventHandler>()
            .As<IIntegrationEventHandler<AddProductPurchasesEvent>>();
    }
}