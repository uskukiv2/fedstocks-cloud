using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Extras;
using fed.cloud.store.domain.Root.Order;
using RepoDb;
using RepoDb.Interfaces;

namespace fed.cloud.store.infrastructure.DataConfigurations
{
    public class OrderConfiguration : IRepoConfiguration
    {
        private readonly IServiceConfiguration _config;

        public OrderConfiguration(IServiceConfiguration configuration)
        {
            _config = configuration;
        }

        public void Configure()
        {
            FluentMapper.Entity<Order>()
                .Table($"[{_config.GetSchema()}].Orders")
                .Primary(x => x.Id, true)
                .DbType(x => x.Id, System.Data.DbType.Guid)
                .Column(x => x.StartedAt, "StartedAt")
                .DbType(x => x.StartedAt, System.Data.DbType.DateTime2)
                .Column(x => x.OrderNumber, "OrderNumber")
                .DbType(x => x.OrderNumber, System.Data.DbType.Int32)
                .Column(x => x.OrderOwner, "OrderOwner")
                .DbType(x => x.OrderOwner, System.Data.DbType.Guid)
                .Column(x => x.StatusId, "Status")
                .DbType(x => x.StatusId, System.Data.DbType.Int16);
        }
    }
}
