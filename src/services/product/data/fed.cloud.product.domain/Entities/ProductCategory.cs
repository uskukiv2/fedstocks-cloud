using fed.cloud.product.domain.Abstraction;

namespace fed.cloud.product.domain.Entities
{
    public class ProductCategory : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public virtual ProductCategory ParentCategory { get; set; }
    }
}