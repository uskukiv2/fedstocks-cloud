using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Order;
using RepoDb;
using System.Data;

namespace fed.cloud.store.infrastructure.DataConfigurations
{
    public class OrderStatusConfiguration : IRepoConfiguration
    {
        public void Configure()
        {
            FluentMapper.Entity<OrderStatus>()
                .Primary(x => x.Id)
                .Identity(x => x.Id)
                .Identity(x => x.Owner)
                .Identity(x => x.StatusId)
                .Column(x => x.StatusId, "StatusId")
                .Column(x => x.Owner, "Owner")
                .Column(x => x.StatusName, "StatusName")
                .Column(x => x.IsCompleteState, "IsCompleteState")
                .DbType(x => x.Owner, DbType.Guid)
                .DbType(x => x.StatusName, DbType.String)
                .DbType(x => x.StatusId, DbType.Int32)
                .DbType(x => x.IsCompleteState, DbType.Boolean);
        }
    }
}