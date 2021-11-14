using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Extras;
using fed.cloud.store.domain.Root.Order;
using RepoDb;
using RepoDb.Interfaces;

namespace fed.cloud.store.infrastructure.DataConfigurations
{
    public class OrderItemConfiguration : IRepoConfiguration
    {
        private readonly IServiceConfiguration _serviceConfiguration;

        public OrderItemConfiguration(IServiceConfiguration serviceConfiguration)
        {
            _serviceConfiguration = serviceConfiguration;
        }

        public void Configure()
        {
            FluentMapper.Entity<OrderItem>()
                .Table($"[{_serviceConfiguration.GetSchema()}].OrderItems")
                .Primary(x => x.Id, true)
                .DbType(x => x.Id, System.Data.DbType.Guid)
                .Column(x => x.OrderId, "OrderId")
                .DbType(x => x.OrderId, System.Data.DbType.Guid)
                .Column(x => x.ItemName, "ItemName")
                .DbType(x => x.ItemName, System.Data.DbType.String)
                .Column(x => x.ProductNumber, "ProductNumber")
                .DbType(x => x.ProductNumber, System.Data.DbType.Int64)
                .Column(x => x.ActualPrice, "Price")
                .DbType(x => x.ActualPrice, System.Data.DbType.Decimal)
                .Column(x => x.Unit, "Unit")
                .DbType(x => x.Unit, System.Data.DbType.Double)
                .Column(x => x.UnitType, "UnitType")
                .PropertyHandler<UnitTypePropertyHandler>("_unitType");
        }
    }

    internal class UnitTypePropertyHandler : IPropertyHandler<int, UnitType>
    {
        public UnitType Get(int input, ClassProperty property)
        {
            return (UnitType)input;
        }

        public int Set(UnitType input, ClassProperty property)
        {
            return (int)input;
        }
    }
}
