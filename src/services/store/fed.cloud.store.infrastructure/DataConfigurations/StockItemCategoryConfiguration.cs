using System.Data;
using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Stock;
using RepoDb;

namespace fed.cloud.store.infrastructure.DataConfigurations
{
    public class StockItemCategoryConfiguration : IRepoConfiguration
    {
        public void Configure()
        {
            FluentMapper.Entity<StockCategory>()
                .Table("StockCategories")
                .Primary(x => x.Id)
                .Identity(x => x.Id)
                .Identity(x => x.OwnerId)
                .Identity(x => x.StockCategoryId)
                .Column(x => x.OwnerId, "OwnerId")
                .Column(x => x.StockCategoryId, "StockCategoryId")
                .Column(x => x.Name, "Name")
                .DbType(x => x.OwnerId, DbType.Guid)
                .DbType(x => x.StockCategoryId, DbType.Int32)
                .DbType(x => x.Name, DbType.String);
        }
    }
}