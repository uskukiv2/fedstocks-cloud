using System;
using System.Collections.Generic;
using fed.cloud.product.domain.Abstraction;
using NpgsqlTypes;

namespace fed.cloud.product.domain.Entities
{
    public class SellerCompany : IEntity
    {
        public Guid Id { get; set; }

        public string OriginalName { get; set; }

        public Guid CountryId { get; set; }

        public Guid CountyId { get; set; }

        public virtual Country Country { get; set; }

        public virtual County County { get; set; }

        public virtual IEnumerable<ProductSellerPrice> SellerPrices { get; set; }

        public virtual NpgsqlTsVector SearchVector { get; set; }
    }
}