namespace fed.cloud.product.application.Models
{
    public class ProductDto
    {
        public string Title { get; set; }

        public string Brand { get; set; }

        public long Number { get; set; }

        public double DefaultQty { get; set; }

        public UnitDto Unit { get; set; }

        public CategoryDto Category { get; set; }

        public ProductSellerPriceDto[] PriceDtos { get; set; }
    }

    public class ProductSellerPriceDto
    {
        public int CurrencyNumber { get; set; }

        public decimal Price { get; set; }
    }

    public class UnitDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Rate { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryDto Parent { get; set; }
    }
}
