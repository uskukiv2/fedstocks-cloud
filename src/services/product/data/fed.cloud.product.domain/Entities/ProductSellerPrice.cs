using fed.cloud.product.domain.Abstraction;
using System;

namespace fed.cloud.product.domain.Entities
{
    public class ProductSellerPrice : IEntity
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid SellerId { get; set; }

        public decimal? Price { get; set; }

        public decimal? OriginalPrice { get; set; }

        public int? OriginalCurrencyNumber { get; set; }

        public virtual Product Product { get; set; }

        public virtual SellerCompany Seller { get; set; }
    }
}