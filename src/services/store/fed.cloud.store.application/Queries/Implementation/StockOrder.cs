using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.store.domain.Root.Stock;
using Npgsql;
using RepoDb;

namespace fed.cloud.store.application.Queries.Implementation;

public class StockQuery : IStockQuery
{
    private readonly string _connectionString;

    public StockQuery(IServiceConfiguration serviceConfiguration)
    {
        _connectionString = serviceConfiguration.Database.Connection;
    }

    public async Task<Stock> GetStockByUserIdAsync(Guid id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var result = await conn.QueryAsync<Stock>("Stocks", new { UserId = id },
            fields: Field.From("Id", "UserId", "Name"));

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<StockItem>> GetStockItemsAsync(string stockName)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var result = await conn.QueryAsync<Stock>("Stocks", new { Name = stockName },
            fields: Field.From("Id", "UserId", "Name"));

        var stockItems = await conn.QueryAsync<StockItem>("StockItems", new { StockId = result.FirstOrDefault()!.Id });

        return stockItems;
    }

    public async Task<Stock> GetGroupStockByGroupIdAsync(Guid groupId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var result = await conn.QueryAsync<Stock>("Stocks", new { GroupId = groupId },
            fields: Field.From("Id", "Name", "GroupId"));

        return result.FirstOrDefault();
    }
}