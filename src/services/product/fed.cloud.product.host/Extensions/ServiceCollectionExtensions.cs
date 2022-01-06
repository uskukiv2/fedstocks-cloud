using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using fed.cloud.common;
using fed.cloud.common.Infrastructure;
using fed.cloud.eventbus;
using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.EventBus;
using fed.cloud.eventbus.EventBus.Abstraction;
using fed.cloud.eventbus.RabbitMq;
using fed.cloud.product.application.Behaviors;
using fed.cloud.product.application.Commands;
using fed.cloud.product.application.IntegrationEvents;
using fed.cloud.product.application.IntegrationEvents.Events;
using fed.cloud.product.application.IntegrationEvents.Handlers;
using fed.cloud.product.application.Queries;
using fed.cloud.product.application.Queries.Implementation;
using fed.cloud.product.domain.Abstraction;
using fed.cloud.product.domain.Repository;
using fed.cloud.product.host.Infrastructure;
using fed.cloud.product.infrastructure;
using fed.cloud.product.infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using RabbitMQ.Client;
using ServiceConfiguration = fed.cloud.product.host.Infrastructure.ServiceConfiguration;

namespace fed.cloud.product.host.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServiceConfigurations(this IServiceCollection service, ConfigurationManager config)
    {
        var serviceConfiguration = new ServiceConfiguration(config.GetDatabaseSchema(), DatabaseType.Ef,
            config.GetDatabaseConnectionString(), config.GetDatabaseDefaultSearchVectorConfig());
        service.AddSingleton<IServiceConfiguration>(serviceConfiguration);

        var eventServiceConfiguration =
            new EventServiceConfiguration(config.GetEventBrokerName(), config.GetLocalEventsSource(), string.Empty);
        service.AddSingleton<IEventServiceConfiguration>(eventServiceConfiguration);
        
        return service;
    }

    internal static IServiceCollection AddCustomDbContext(this IServiceCollection service, ConfigurationManager configuration)
    {
        service.AddEntityFrameworkNpgsql()
            .AddDbContext<ProductContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetDatabaseConnectionString(), options =>
                {
                    options.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                });
            });
        service.AddScoped<IUnitOfWork<NpgsqlConnection>>(sp => sp.GetRequiredService<ProductContext>());
        return service;
    }

    internal static IServiceCollection AddIntegrationEvent(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddTransient<IIntegrationEventLogService>(sp =>
            new IntegrationEventLogService(sp.GetRequiredService<IEventServiceConfiguration>()));

        service.AddTransient<IProductIntegrationEventService, ProductIntegrationEventService>();

        var eventSettings = config.GetFullEventsSection();

        service.AddSingleton<IRabbitMqClient>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = eventSettings.HostName,
                DispatchConsumersAsync = true,
                UserName = eventSettings.HostLogin,
                Password = eventSettings.HostPassword
            };

            return new DefaultRabbitMqClient(factory, sp.GetRequiredService<ILogger<DefaultRabbitMqClient>>(),
                eventSettings.MaxRetryAllowed, eventSettings.MaxTimeout);
        });

        return service;
    }

    internal static IServiceCollection AddEventBus(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
        {
            var rabbitMq = sp.GetRequiredService<IRabbitMqClient>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
            var eventBusSubscribeManager = sp.GetRequiredService<IEventBusSubscribeManager>();
            var eventServiceConfiguration = sp.GetRequiredService<IEventServiceConfiguration>();
            var handlerResolver = sp.GetRequiredService<IHandlerResolver>();
            var queueName = config.GetEventQueueName();

            return new EventBusRabbitMq(eventBusSubscribeManager, rabbitMq, eventServiceConfiguration, handlerResolver, queueName, logger);
        });

        service.AddSingleton<IHandlerResolver, EventHandlerResolver>();
        service.AddSingleton<IEventBusSubscribeManager, InMemoryEventBusSubscriptionManager>();
        return service;
    }
}