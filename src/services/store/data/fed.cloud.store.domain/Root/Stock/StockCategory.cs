using System;

namespace fed.cloud.store.domain.Root.Stock
{
    public class StockCategory
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public int StockCategoryId { get; set; }

        public string Name { get; set; }

        public static StockCategory Create(Guid ownerId, int nextCategoryId, string name)
        {
            return new StockCategory
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                StockCategoryId = nextCategoryId,
                Name = name
            };
        }
    }
}