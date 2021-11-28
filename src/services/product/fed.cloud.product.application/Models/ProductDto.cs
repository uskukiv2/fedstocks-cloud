using System;
using System.Collections.Generic;
using System.Text;

namespace fed.cloud.product.application.Models
{
    public class ProductDto
    {
        public string Title { get; set; }

        public string Brand { get; set; }

        public long Number { get; set; }

        public double DefaultQty { get; set; }

        public int Unit { get; set; }

        public int Category { get; set; }

        public ProductSellerPriceDto[] SellerPrices { get; set; }
    }

    public class ProductSellerPriceDto
    {
        public Guid SellerId { get; set; }

        public int CurrencyNumber { get; set; }

        public decimal Price { get; set; }
    }
}
