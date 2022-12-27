using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Stock;
using RepoDb;
using System.Data;

namespace fed.cloud.store.infrastructure.DataConfigurations
{
    public class StockConfiguration : IRepoConfiguration
    {
        public void Configure()
        {
            FluentMapper.Entity<Stock>()
                .Table("Stocks")
                .Primary(x => x.Id, true)
                .Column(x => x.GroupId, "GroupId")
                .Column(x => x.Name, "Name")
                .Column(x => x.UserId, "UserId")
                .Column(x => x.IsDefault, "IsDefault")
                .DbType(x => x.GroupId, DbType.Guid)
                .DbType(x => x.UserId, DbType.Guid)
                .DbType(x => x.Name, DbType.String)
                .DbType(x => x.IsDefault, DbType.Boolean);
        }
    }
}