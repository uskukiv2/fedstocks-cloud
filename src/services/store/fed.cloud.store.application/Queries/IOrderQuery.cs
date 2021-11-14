using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fed.cloud.store.domain.Root.Order;

namespace fed.cloud.store.application.Queries;

public interface IOrderQuery
{
    Task<Order> GetOrderAsync(string orderNumber);
    Task<Order> GetOrderAsync(Guid id);
    Task<IEnumerable<Order>> GetOrdersByOwnerIdAsync(Guid ownerId);
    Task<IEnumerable<OrderStatus>> GetAvailableStatuses(Guid userId);
}