using fed.cloud.product.domain.Abstraction;
using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace fed.cloud.product.domain.Entities
{
    public class Product : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string Manufacturer { get; set; }

        public long GlobalNumber { get; set; }

        public int CategoryId { get; set; }

        public double QuantityRate { get; set; }

        public int UnitId { get; set; }

        public virtual ProductCategory Category { get; set; }

        public virtual ProductUnit Unit { get; set; }

        public virtual IEnumerable<ProductSellerPrice> SellerPrices { get; set; }

        public virtual NpgsqlTsVector SearchVector { get; set; }
    }
}