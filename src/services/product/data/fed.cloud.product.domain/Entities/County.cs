using fed.cloud.product.domain.Abstraction;
using System;

namespace fed.cloud.product.domain.Entities
{
    public class County : IEntity
    {
        public Guid Id { get; set; }

        public int NumberInCountry { get; set; }

        public string Name { get; set; }
    }
}