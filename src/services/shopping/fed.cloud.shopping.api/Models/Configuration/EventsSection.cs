namespace fed.cloud.shopping.api.Models.Configuration;

public class EventsSection
{
    public string QueueName { get; set; }

    public string BrokerName { get; set; }

    public string LocalEventsSource { get; set; }

    public string HostName { get; set; }

    public string HostLogin { get; set; }

    public string HostPassword { get; set; }

    public int MaxRetryAllowed { get; set; }

    public int MaxTimeout { get; set; }
}