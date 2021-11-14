namespace fed.cloud.store.application.Models;

public record OrderLineData
{
    public int ProductNumber { get; set; }

    public string Name { get; set; }

    public string UnitString { get; set; }

    public decimal TotalPerStandart { get; set; }

    public decimal TotalPerUnit { get; set; }

    public double TotalUnit { get; set; }
}