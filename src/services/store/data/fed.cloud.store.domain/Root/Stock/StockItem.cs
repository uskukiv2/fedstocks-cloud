using System;

namespace fed.cloud.store.domain.Root.Stock
{
    public class StockItem
    {
        public StockItem()
        {

        }

        private StockItem(long number,
                          string name,
                          double quantity,
                          int categoryId,
                          int unitId,
                          Guid stockId)
        {
            Id = Guid.NewGuid();
            Number = number;
            Name = name;
            Quantity = quantity;
            CategoryId = categoryId;
            UnitId = unitId;
            StockId = stockId;
        }

        public Guid Id { get; }

        public long Number { get; }

        public string Name { get; }

        public double Quantity { get; set; }

        public int CategoryId { get; }

        public int UnitId { get; }

        public Guid StockId { get; }

        public StockCategory Category { get; }

        public UnitType Unit { get; }

        internal static StockItem Create(long number,
                                         string name,
                                         double quantiy,
                                         int categoryId,
                                         int unitId,
                                         Guid stockId)
        {
            return new StockItem(number,
                                 name,
                                 quantiy,
                                 categoryId,
                                 unitId,
                                 stockId);
        }
    }
}