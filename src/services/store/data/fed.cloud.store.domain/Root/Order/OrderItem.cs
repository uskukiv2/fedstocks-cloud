using fed.cloud.store.domain.Extras;
using System;

namespace fed.cloud.store.domain.Root.Order
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string ItemName { get; set; }

        public long ProductNumber { get; set; }

        public decimal ActualPrice { get; set; }

        public decimal OriginalPrice { get; set; }

        public double Unit { get; set; }

        public UnitType UnitType { get; set; }

        /// <summary>
        /// Increasing current unit on one item
        /// </summary>
        public void IncreaseUnit()
        {
            Unit++;
        }

        public static OrderItem Create(Guid orderId, string lineName, long productNumber, decimal price, UnitType unitType, double unit)
        {
            return new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ItemName = lineName,
                ProductNumber = productNumber,
                ActualPrice = price,
                UnitType = unitType,
                Unit = unit
            };
        }

        public static OrderItem Reconstruct(Guid orderId,
                                            Guid orderItemId,
                                            string itemName,
                                            long productNumber,
                                            decimal price,
                                            double unit,
                                            UnitType unitType)
        {
            return new OrderItem
            {
                Id = orderItemId,
                OrderId = orderId,
                ItemName = itemName,
                ProductNumber = productNumber,
                ActualPrice = price,
                UnitType = unitType,
                Unit = unit
            };
        }
    }
}
