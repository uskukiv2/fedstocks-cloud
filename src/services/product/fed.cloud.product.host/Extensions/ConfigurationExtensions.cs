using fed.cloud.product.host.Models.Configurations;

namespace fed.cloud.product.host.Extensions;

internal static class ConfigurationExtensions
{
    internal static string GetDatabaseSchema(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("DatabaseSection").Get<DatabaseSection>().Schema;
        }
        catch
        {
            return "product";
        }
    }

    internal static string GetDatabaseConnectionString(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("DatabaseSection").Get<DatabaseSection>().ConnectionString;
        }
        catch
        {
            return "Host=localhost;Username=prod_user;Password=ProductQwerty123;Database=db_products";
        }
    }

    internal static string GetDatabaseDefaultSearchVectorConfig(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("DatabaseSection").Get<DatabaseSection>().DefaultSearchVectorConfig;
        }
        catch
        {
            return "english";
        }
    }

    internal static string GetEventBrokerName(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("EventSection").Get<EventsSection>().BrokerName;
        }
        catch
        {
            return "fed_broker";
        }
    }

    internal static string GetLocalEventsSource(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("EventSection").Get<EventsSection>().LocalEventsSource;
        }
        catch
        {
            return @"events.db";
        }
    }

    internal static string GetEventQueueName(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("EventSection").Get<EventsSection>().QueueName;
        }
        catch
        {
            return "fed_products_queue";
        }
    }

    internal static EventsSection GetFullEventSection(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("EventSection").Get<EventsSection>();
        }
        catch
        {
            return new EventsSection()
            {
                BrokerName = "fed_broker",
                HostName = "localhost",
                HostLogin = "guest",
                HostPassword = "guest",
                LocalEventsSource = "events.db",
                MaxRetryAllowed = 5,
                MaxTimeout = 10000
            };
        }
    }
}