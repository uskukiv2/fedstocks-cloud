namespace fedstocks.cloud.web.api.Models;

public class Product
{
    public long Number { get; set; }

    public string Brand { get; set; }

    public string Name { get; set; }

    public decimal UnitPrice { get; set; }

    public Unit Unit { get; set; }

    public Category Category { get; set; }
}

public class Category
{
    public string Name { get; set; }

    public Category ParentCategory { get; set; }
}