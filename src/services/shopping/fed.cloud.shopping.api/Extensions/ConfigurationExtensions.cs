using fed.cloud.shopping.api.Models.Configuration;

namespace fed.cloud.product.host.Extensions;

internal static class ConfigurationExtensions
{
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