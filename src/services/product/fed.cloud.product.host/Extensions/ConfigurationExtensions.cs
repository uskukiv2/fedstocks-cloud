using fed.cloud.common.Models;
using fed.cloud.product.host.Models.Configurations;

namespace fed.cloud.product.host.Extensions;

internal static class ConfigurationExtensions
{
    internal static int GetPort(this ConfigurationManager configuration)
    {
        return int.Parse(configuration.GetSection("Port").Value);
    }

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
            return configuration.GetSection("EventsSection").Get<EventsSection>().BrokerName;
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
            return configuration.GetSection("EventsSection").Get<EventsSection>().LocalEventsSource;
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
            return configuration.GetSection("EventsSection").Get<EventsSection>().QueueName;
        }
        catch
        {
            return "fed_products_queue";
        }
    }

    internal static EventsSection GetFullEventsSection(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("EventsSection").Get<EventsSection>();
        }
        catch
        {
            return new EventsSection()
            {
                BrokerName = "fed_broker",
                HostName = "localhost",
                HostLogin = "guest",
                HostPassword = "guest",
                HostPort = 5672,
                LocalEventsSource = "events.db",
                MaxRetryAllowed = 5,
                MaxTimeout = 10000,
                QueueName = "fed_product"
            };
        }
    }

    internal static string GetServicesCertPath(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("Certification").Get<Certification>().CertificatePath;
        }
        catch
        {
            return "f_service.pem";
        }
    }

    internal static string GetServiceKeyCertPath(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("Certification").Get<Certification>().KeyCertificatePath;
        }
        catch
        {
            return "f_key";
        }
    }
    
    internal static LogLevel GetDefaultLogLevel(this ConfigurationManager configuration)
    {
        return configuration.GetSection("Logging").GetSection("LogLevel").GetValue<LogLevel>("Default");
    }

    internal static bool GetRegenerateDatabase(this IConfiguration configuration)
    {
        return configuration.GetSection("Specials").GetValue<bool>("RegenerateDatabase");
    }
}