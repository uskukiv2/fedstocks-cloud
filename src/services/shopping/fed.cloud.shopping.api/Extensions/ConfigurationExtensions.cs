using fed.cloud.common.Models;
using fed.cloud.shopping.api.Models.Configuration;

namespace fed.cloud.shopping.api.Extensions;

internal static class ConfigurationExtensions
{
    internal static string GetUrls(this ConfigurationManager configuration)
    {
        return configuration.GetSection("Urls").Value;
    }

    internal static Redis GetRedisSection(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("Redis").Get<Redis>();
        }
        catch
        {
            return new Redis();
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
                LocalEventsSource = "events.db",
                MaxRetryAllowed = 5,
                MaxTimeout = 10000
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

    internal static int GetPort(this ConfigurationManager configuration)
    {
        return int.Parse(configuration.GetSection("Port").Value);
    }

    internal static LogLevel GetDefaultLogLevel(this ConfigurationManager configuration)
    {
        return configuration.GetSection("Logging").GetSection("LogLevel").GetValue<LogLevel>("Default");
    }
}