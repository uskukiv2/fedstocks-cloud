using fed.cloud.product.domain.Abstraction;

namespace fed.cloud.product.domain.Entities
{
    public class ProductUnit : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Rate { get; set; }
    }
}