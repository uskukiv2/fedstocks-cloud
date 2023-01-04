using fed.cloud.eventbus.Base;

namespace fed.cloud.shopping.api.Application.IntegrationEvents.Events
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

        public decimal OriginalPrice { get; set; }

        public Guid Seller { get; set; }

        public int UnitId { get; set; }

        public int CategoryId { get; set; }

    }
}