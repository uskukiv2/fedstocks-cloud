namespace fed.cloud.product.host.Models.Configurations;

public class DatabaseSection
{
    public string Schema { get; set; }

    public string ConnectionString { get; set; }

    public string DefaultSearchVectorConfig { get; set; }
}