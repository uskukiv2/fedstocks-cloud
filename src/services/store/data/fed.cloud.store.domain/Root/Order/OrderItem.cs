using System;
using fed.cloud.store.domain.Abstract;
using fed.cloud.store.domain.Extras;

namespace fed.cloud.store.domain.Root.Order
{
    public class OrderItem
    {
        private Guid _orderId;
        private Guid _orderItemId;
        private string _itemName;
        private Guid _productId;
        private decimal _price;
        private double _unit;
        private UnitType _unitType;

        private OrderItem(Guid orderId, string lineName, Guid productId, decimal price, UnitType unitType, double unit)
        {
            _orderId = orderId;
            _orderItemId = Guid.NewGuid();
            _itemName = lineName;
            _productId = productId;
            _price = price;
            _unit = unit;
            _unitType = unitType;
        }

        public OrderItem(Guid orderId,
                         Guid orderItemId,
                         string itemName,
                         Guid productId,
                         decimal price,
                         double unit,
                         UnitType unitType)
        {
            _orderId = orderId;
            _orderItemId = orderItemId;
            _itemName = itemName;
            _productId = productId;
            _price = price;
            _unit = unit;
            _unitType = unitType;
        }

        public Guid ProductId => _productId;

        /// <summary>
        /// Increasing current unit on one item
        /// </summary>
        public void IncreaseUnit()
        {
            _unit++;
        }

        public string GetName() => _itemName;

        public decimal GetPrice() => _price;

        public double GetUnit() => _unit;

        public UnitType GetUnitType() => _unitType;

        public static OrderItem Create(Guid orderId, string lineName, Guid productId, decimal price, UnitType unitType, double unit)
        {
            return new OrderItem(orderId, lineName, productId, price, unitType, unit);
        }

        public static OrderItem Reconstruct(Guid orderId,
                                            Guid orderItemId,
                                            string itemName,
                                            Guid productId,
                                            decimal price,
                                            double unit,
                                            UnitType unitType)
        {
            return new OrderItem(orderId, orderItemId, itemName, productId, price, unit, unitType);
        }
    }
}
