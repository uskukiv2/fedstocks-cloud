using fed.cloud.eventbus;
using fed.cloud.eventbus.Base;
using fed.cloud.eventbus.EventBus;
using fed.cloud.eventbus.EventBus.Abstraction;
using fed.cloud.eventbus.RabbitMq;
using fed.cloud.product.host.Extensions;
using fed.cloud.shopping.api.Application.IntegrationEvents;
using fed.cloud.shopping.domain.Repositories;
using fed.cloud.shopping.infrastructure;
using fed.cloud.shopping.infrastructure.Infrastructure;
using fed.cloud.shopping.infrastructure.Repository;
using RabbitMQ.Client;

namespace fed.cloud.shopping.api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddServiceConfigurations(this IServiceCollection service, ConfigurationManager config)
    {
        var eventServiceConfiguration =
            new EventServiceConfiguration(config.GetEventBrokerName(), config.GetLocalEventsSource(), string.Empty);
        service.AddSingleton<IEventServiceConfiguration>(eventServiceConfiguration);
        
        return service;
    }

    internal static IServiceCollection AddRedis(this IServiceCollection service, ConfigurationManager configuration)
    {
        var redisConfig = configuration.GetRedisSection();
        service.AddSingleton<ICacheClient>(sp => new CacheClient(redisConfig.Database.Host, redisConfig.Database.Port,
            redisConfig.User.Username, redisConfig.User.Password, redisConfig.Database.Name));

        return service;
    }

    internal static IServiceCollection AddIntegrationEvent(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddTransient<IIntegrationEventLogService>(sp =>
            new IntegrationEventLogService(sp.GetRequiredService<IEventServiceConfiguration>()));

        service.AddTransient<IShoppingIntegrationEventService, ShoppingIntegrationEventService>();

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

    internal static IServiceCollection AddDbOperations(this IServiceCollection service)
    {
        service.AddScoped<IShoppingListRepository, RedisShoppingListRepository>();

        return service;
    }
}