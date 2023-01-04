using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Order;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fed.cloud.store.application.Queries.Implementation;

public class OrderQuery : IOrderQuery
{
    private readonly string _connection;

    public OrderQuery(IServiceConfiguration config)
    {
        _connection = config.Database.Connection;
    }

    public async Task<Order> GetOrderAsync(string orderNumber)
    {
        return await InternalGetOrderAsync(new { OrderNumber = orderNumber });
    }

    public async Task<Order> GetOrderAsync(Guid id)
    {
        return await InternalGetOrderAsync(new { Id = id });
    }

    public async Task<IEnumerable<Order>> GetOrdersByOwnerIdAsync(Guid ownerId)
    {
        await using var conn = new NpgsqlConnection(_connection);
        var orders = (await conn
            .QueryAsync<Order>(
                "Orders", new { OrderOwner = ownerId }, top: 10)).ToList();
        foreach (var item in orders)
        {
            var orderItems = await conn.QueryAsync<OrderItem>("OrderItems",
                new { item.Id });

            orderItems.ToList().ForEach(x =>
                item.ReconstructAndAddLine(x.Id, x.ItemName, x.ProductNumber, x.ActualPrice, x.Unit, x.UnitType));
            orders.Add(item);
        }

        return orders;
    }

    public async Task<IEnumerable<OrderStatus>> GetAvailableStatuses(Guid userId)
    {
        await using var connection = new NpgsqlConnection(_connection);
        var orderStatuses = await connection.QueryAsync<OrderStatus>("OrderStatuses", new { Owner = userId });

        return orderStatuses;
    }

    private async Task<Order> InternalGetOrderAsync(object what)
    {
        await using var conn = new NpgsqlConnection(_connection);
        var order = (await conn
                .QueryAsync<Order>(
                    "Orders", what))
            .FirstOrDefault()!;
        var orderStatus = (await conn.QueryAsync<OrderStatus>("OrderStatuses", new { Id = order.StatusId })).FirstOrDefault()!;
        order.Status = orderStatus;
        var orderItems = await conn.QueryAsync<OrderItem>("OrderItems", new { order.Id });

        orderItems.ToList().ForEach(x =>
            order.ReconstructAndAddLine(x.Id, x.ItemName, x.ProductNumber, x.ActualPrice, x.Unit, x.UnitType));
        return order;
    }
}