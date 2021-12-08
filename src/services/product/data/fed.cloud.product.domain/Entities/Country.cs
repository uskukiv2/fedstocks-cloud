using fed.cloud.product.domain.Abstraction;
using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace fed.cloud.product.domain.Entities
{
    public class Country : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int GlobalId { get; set; }

        public virtual IEnumerable<County> Counties { get; set; }

        public virtual NpgsqlTsVector SearchVector { get; set; }
    }
}