using System;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.store.domain.Abstract;

namespace fed.cloud.store.domain.Root.Order
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Add(Order order, CancellationToken token = default(CancellationToken));
        void Update(Order order, CancellationToken token = default(CancellationToken)); 
        Task<Order> GetAsync(string orderNumber);
        Task<Order> GetAsync(Guid orderId);
    }
}
