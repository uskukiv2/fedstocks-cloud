using System;
using fed.cloud.product.domain.Abstraction;

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
    }
}