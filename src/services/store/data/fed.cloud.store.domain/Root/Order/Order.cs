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
        private Guid _orderId;
        private DateTime _orderDate;
        private int _receiptNumber;
        private string _orderNumber;
        private Guid _orderOwner;
        private StatusType _status;
        private string _statusInfo;

        protected Order()
        {
            _items = new List<OrderItem>();
        }

        protected Order(Guid ownerId, int receiptNumber, string orderNumber, DateTime orderDate) : base()
        {
            _orderId = Guid.NewGuid();
            _orderOwner = ownerId;
            _receiptNumber = receiptNumber;
            _orderNumber = orderNumber;
            _orderDate = orderDate;
            _statusInfo = string.Empty;
            _status = StatusType.New;
        }

        public Order(Guid orderId, DateTime orderDate, int receiptNumber, Guid orderOwner, StatusType status, string info) : base()
        {
            _orderId = orderId;
            _orderDate = orderDate;
            _receiptNumber = receiptNumber;
            _orderOwner = orderOwner;
            _status = status;
            _statusInfo = info;
        }

        public IReadOnlyCollection<OrderItem> Items => _items;

        public DateTime GetOrderDate() => _orderDate;
        public int GetReceiptNumber() => _receiptNumber;
        public string GetOrderNumber() => _orderNumber;
        public Guid GetOrderOwner() => _orderOwner;
        public StatusType GetOrderStatus() => _status;
        public string GetOrderStatusInfo() => _statusInfo;

        public void AddLine(string lineName, Guid productId, decimal price, UnitType unitType, double unit)
        {
            var existOrderItem = _items.FirstOrDefault(x => x.ProductId == productId);

            if (existOrderItem != null)
            {
                existOrderItem.IncreaseUnit();
                return;
            }

            _items.Add(OrderItem.Create(_orderId, lineName, productId, price, unitType, unit));
        }

        public void ReconstructAndAddLine(Guid orderItemId,
                                            string itemName,
                                            Guid productId,
                                            decimal price,
                                            double unit,
                                            UnitType unitType)
        {
            var existOrderItem = _items.FirstOrDefault(x => x.ProductId == productId);

            if (existOrderItem != null)
            {
                existOrderItem.IncreaseUnit();
                return;
            }

            _items.Add(OrderItem.Reconstruct(_orderId, orderItemId, itemName, productId, price, unit, unitType));
        }

        public void UpdateStatus(StatusType status, string info)
        {
            _status = status;
            _statusInfo = info;
        }

        public static Order Reconstruct(Guid orderId,
                                        DateTime orderDate,
                                        int receiptNumber,
                                        Guid orderOwner,
                                        StatusType status,
                                        string info)
        {
            return new Order(orderId, orderDate, receiptNumber, orderOwner, status, info);
        }

        private static string CreateOrderNumber(DateTime time, int number)
        {
            var todaysOrderNumber = Math.Abs(number) % 100;

            return $"{time.Year}({time.Month}{time.Day})-{time.Hour}.{time.Minute} {todaysOrderNumber}";
        }

        public static Order CreateNewOrder(Guid owner, int receiptNumber, DateTime orderDate)
        {
            return new Order(owner, receiptNumber, CreateOrderNumber(orderDate, receiptNumber), orderDate);
        }
    }
}
