using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Order;
using fed.cloud.store.infrastructure.Factories;
using Npgsql;
using RepoDb;

namespace fed.cloud.store.infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order, NpgsqlConnection>, IOrderRepository
    {
        private readonly IPortableRepository<OrderStatus> _orderStatusRepository;
        private readonly IPortableRepository<OrderItem> _orderItemRepository;

        public OrderRepository(IServiceConfiguration configuration, IUnitOfWork unitOfWork,
            IRepositoryFactory<NpgsqlConnection> repositoryFactory)
            : base(configuration.Database.Connection)
        {
            UnitOfWork = unitOfWork;
            _orderStatusRepository = repositoryFactory.Create<OrderStatus>();
            _orderItemRepository = repositoryFactory.Create<OrderItem>();
        }

        public IUnitOfWork UnitOfWork { get; }

        public void Add(Order order, CancellationToken token = default)
        {
            InsertAsync<Order>(order, transaction: UnitOfWork.Transaction, cancellationToken: token);
        }

        public async Task<Order> GetAsync(string orderNumber)
        {
            var query = new QueryField("OrderNumber", orderNumber);
            var order = (await QueryAsync(query, transaction: UnitOfWork.Transaction)).FirstOrDefault()!;
            var orderStatus =
                (await _orderStatusRepository.QueryAsync("OrderStatuses", new {Id = order.StatusId},
                    CancellationToken.None)).FirstOrDefault()!;
            order.Status = orderStatus;

            return order;
        }

        public async Task<OrderStatus> GetCompleteOrderStatusAsync(Guid owner)
        {
            var resultOrderStatus = await _orderStatusRepository.QueryAsync("OrderStatus",
                new { Owner = owner, IsCompleteState = true }, CancellationToken.None);

            return resultOrderStatus.FirstOrDefault()!;
        }

        public async Task<OrderStatus> GetStatusAsync(Guid owner, int orderStatusId)
        {
            var resultOrderStatus = await _orderStatusRepository.QueryAsync("OrderStatus",
                new { Owner = owner, StatusId = orderStatusId }, CancellationToken.None);

            return resultOrderStatus.FirstOrDefault()!;
        }

        public async Task<Order> GetAsync(Guid orderId)
        {
            var query = new QueryField("OrderId", orderId);
            var order = (await QueryAsync(query, transaction: UnitOfWork.Transaction)).SingleOrDefault()!;

            var orderStatus =
                (await _orderStatusRepository.QueryAsync("OrderStatuses", new { Id = order.StatusId },
                    CancellationToken.None)).FirstOrDefault();
            order.Status = orderStatus!;

            var orderItems =
                await _orderItemRepository.QueryAsync("OrderItems", new { OrderId = order.Id }, CancellationToken.None);
            orderItems.ToList().ForEach(x =>
                order.ReconstructAndAddLine(x.Id, x.ItemName, x.ProductNumber, x.ActualPrice, x.Unit, x.UnitType));

            return order;
        }

        public void Update(Order order, CancellationToken token = default)
        {
            UpdateAsync(order, transaction: UnitOfWork.Transaction, cancellationToken: token);
        }
    }
}
