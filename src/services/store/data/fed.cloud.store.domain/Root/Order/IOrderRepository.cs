using System;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.store.domain.Abstract;

namespace fed.cloud.store.domain.Root.Order
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetAsync(string orderNumber);
        Task<OrderStatus> GetCompleteOrderStatusAsync(Guid owner);
        Task<OrderStatus> GetStatusAsync(Guid owner, int orderStatusId);
    }
}
