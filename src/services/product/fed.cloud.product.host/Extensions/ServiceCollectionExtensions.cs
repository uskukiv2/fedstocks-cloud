using System.Reflection;
using fed.cloud.common;
using fed.cloud.common.Infrastructure;
using fed.cloud.eventbus;
using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.EventBus;
using fed.cloud.eventbus.EventBus.Abstraction;
using fed.cloud.eventbus.RabbitMq;
using fed.cloud.product.application.Behaviors;
using fed.cloud.product.application.IntegrationEvents;
using fed.cloud.product.application.IntegrationEvents.Handlers;
using fed.cloud.product.host.Infrastructure;
using fed.cloud.product.infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        return service;
    }

    internal static IServiceCollection AddIntegrationEvent(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddTransient<IIntegrationEventLogService>(sp =>
            new IntegrationEventLogService(sp.GetRequiredService<IEventServiceConfiguration>()));

        service.AddTransient<IProductIntegrationEventService, ProductIntegrationEventService>();

        var eventSettings = config.GetFullEventSection();

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
        service.AddTransient<AddProductPurchasesEventHandler>();
        return service;
    }

    internal static IServiceCollection AddMediator(this IServiceCollection service)
    {
        service.AddMediatR(typeof(Program));

        service.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        service.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        service.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return service;
    }
}