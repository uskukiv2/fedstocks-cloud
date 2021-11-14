using System;
using System.Collections.Generic;
using System.Linq;
using fed.cloud.store.domain.Abstract;
using fed.cloud.store.domain.Extras;

namespace fed.cloud.store.domain.Root.Order
{
    public class Order : IRoot
    {
        private readonly List<OrderItem> _items;

        private Order()
        {
            _items = new List<OrderItem>();
        }

        public Guid Id { get; set; }

        public DateTime StartedAt { get; set; }

        public string OrderNumber { get; set; }

        public Guid OrderOwner { get; set; }

        public Guid StatusId { get; set; }

        public OrderStatus Status { get; set; }

        public IReadOnlyCollection<OrderItem> Items => _items;

        public void AddLine(string lineName, long productNumber, decimal price, UnitType unitType, double unit)
        {
            var existOrderItem = _items.FirstOrDefault(x => x.ProductNumber == productNumber);

            if (existOrderItem != null)
            {
                existOrderItem.IncreaseUnit();
                return;
            }

            _items.Add(OrderItem.Create(Id, lineName, productNumber, price, unitType, unit));
        }

        public void ReconstructAndAddLine(Guid orderItemId,
                                            string itemName,
                                            long productNumber,
                                            decimal price,
                                            double unit,
                                            UnitType unitType)
        {
            var existOrderItem = _items.FirstOrDefault(x => x.ProductNumber == productNumber);

            if (existOrderItem != null)
            {
                existOrderItem.IncreaseUnit();
                return;
            }

            _items.Add(OrderItem.Reconstruct(Id, orderItemId, itemName, productNumber, price, unit, unitType));
        }

        public static Order Reconstruct(Guid orderId,
                                        DateTime orderDate,
                                        string receiptNumber,
                                        Guid orderOwner)
        {
            return new Order
            {
                Id = orderId,
                StartedAt = orderDate,
                OrderNumber = receiptNumber,
                OrderOwner = orderOwner
            };
        }

        private static string CreateOrderNumber(DateTime time, int number)
        {
            var todaysOrderNumber = Math.Abs(number) % 100;

            return $"{time.Year}({time.Month}{time.Day})-{time.Hour}.{time.Minute} {todaysOrderNumber}";
        }

        public static Order CreateNewOrder(Guid owner, string receiptNumber, DateTime orderDate)
        {
            return new Order() 
            {
                Id = Guid.NewGuid(),
                OrderOwner = owner,
                OrderNumber = receiptNumber,
                StartedAt = orderDate,
            };
        }

        public static Order CreateEmptyOrder()
        {
            return new Order()
            {
                Id = Guid.Empty
            };
        }
    }
}
