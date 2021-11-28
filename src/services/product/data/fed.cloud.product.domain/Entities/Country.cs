using fed.cloud.product.domain.Abstraction;
using System;

namespace fed.cloud.product.domain.Entities
{
    public class Country : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int GlobalId { get; set; }
    }
}