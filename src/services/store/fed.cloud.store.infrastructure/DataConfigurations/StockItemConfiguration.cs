using System.Data;
using System.Data.Common;
using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Stock;
using RepoDb;
using RepoDb.Interfaces;

namespace fed.cloud.store.infrastructure.DataConfigurations
{
    public class StockItemConfiguration : IRepoConfiguration
    {
        public void Configure()
        {
            FluentMapper.Entity<StockItem>()
                .Table("StockItems")
                .Primary(x => x.Id)
                .Column(x => x.Name, "Name")
                .Column(x => x.Number, "Number")
                .Column(x => x.Quantity, "Quantity")
                .Column(x => x.StockId, "StockId")
                .Column(x => x.CategoryId, "CategoryId")
                .PropertyHandler<UnitTypePropertyHandler>(x => x.Unit)
                .DbType(x => x.Name, DbType.String)
                .DbType(x => x.Number, DbType.Int64)
                .DbType(x => x.Quantity, DbType.Int32)
                .DbType(x => x.StockId, DbType.Guid)
                .DbType(x => x.CategoryId, DbType.Int32);
        }
    }
}