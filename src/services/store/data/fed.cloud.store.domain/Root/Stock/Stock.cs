using System;
using System.Collections.Generic;
using fed.cloud.store.domain.Abstract;

namespace fed.cloud.store.domain.Root.Stock
{
    public class Stock : IRoot
    {
        private readonly List<StockItem> _stockItems;

        public Stock()
        {
            Id = Guid.NewGuid();
            _stockItems = new List<StockItem>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; }

        public Guid GroupId { get; set; }

        public IReadOnlyCollection<StockItem> StockItems => _stockItems;

        public void RemoveStockItem(Guid id)
        {
            var stockItemToRemove = _stockItems.Find(x => x.Id == id);
            _stockItems.Remove(stockItemToRemove);
        }

        public void AddStockItem(long number, string productName, int categoryId, int unitId, double qty)
        {
            if (number < 1) throw new ArgumentOutOfRangeException(nameof(number));
            if (string.IsNullOrEmpty(productName)) throw new ArgumentOutOfRangeException(nameof(number));
            if (categoryId < 1) throw new ArgumentOutOfRangeException(nameof(categoryId));
            if (unitId < 1) throw new ArgumentOutOfRangeException(nameof(unitId));
            if (qty < 1) throw new ArgumentOutOfRangeException(nameof(qty));

            _stockItems.Add(StockItem.Create(number,
                                             Name,
                                             qty,
                                             categoryId,
                                             unitId,
                                             Id));
        }
    }
}