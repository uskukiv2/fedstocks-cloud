using System;
using fed.cloud.eventbus.Base;

namespace fed.cloud.product.application.IntegrationEvents.Events
{
    public record AddProductPurchasesEvent(DateTime BoughtDate, BoughtProduct[] Lines) : IntegrationEvent
    {
        public DateTime BoughtDate { get; } = BoughtDate;

        public BoughtProduct[] Lines { get; } = Lines;
    }

    public record BoughtProduct
    {
        public long Number { get; set; }

        public string Brand { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal OriginalPrice { get; set; }

        public string Seller { get; set; }
        
        public int UnitId { get; set; }
        
        public int CategoryId { get; set; }
    }
}