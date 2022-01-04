namespace fed.cloud.product.host.Models.Configurations;

public class EventsSection
{
    public string QueueName { get; set; }

    public string BrokerName { get; set; }

    public string LocalEventsSource { get; set; }

    public string HostName { get; set; }

    public int HostPort { get; set; }

    public string HostLogin { get; set; }

    public string HostPassword { get; set; }

    public int MaxRetryAllowed { get; set; }

    public int MaxTimeout { get; set; }
}