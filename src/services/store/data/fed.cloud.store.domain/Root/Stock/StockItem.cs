using fed.cloud.store.domain.Extras;
using System;

namespace fed.cloud.store.domain.Root.Stock
{
    public class StockItem
    {
        public Guid Id { get; set; }

        public long Number { get; set; }

        public string Name { get; set; }

        public double Quantity { get; set; }

        public int CategoryId { get; set; }

        public UnitType Unit { get; set; }

        public Guid StockId { get; set; }

        public StockCategory Category { get; set; }

        internal static StockItem Create(long number,
                                         string name,
                                         double quantity,
                                         int categoryId,
                                         UnitType unitType,
                                         Guid stockId)
        {
            return new StockItem
            {
                Id = Guid.NewGuid(),
                Number = number,
                Name = name,
                Quantity = quantity,
                CategoryId = categoryId,
                Unit = unitType,
                StockId = stockId
            };
        }
    }
}