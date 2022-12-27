using fed.cloud.common.Infrastructure;
using fed.cloud.product.application.Models;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fed.cloud.product.application.Queries.Implementation;

public class SellerQuery : ISellerQuery
{
    private readonly string _connection;

    public SellerQuery(IServiceConfiguration configuration)
    {
        _connection = configuration.Database.Connection;
    }

    public async Task<IEnumerable<SellerSummaryDto>> GetSellersAsync(Guid countryId, int countyNumber)
    {
        await using var conn = new NpgsqlConnection(_connection);

        var county = (await conn.QueryAsync("product.counties", new { CountryId = countryId, Number = countyNumber },
            Field.From("Id", "Name"))).FirstOrDefault();

        var counties = await conn.QueryAsync("product.sellers", new { CountryId = countryId, CountyId = county.Id },
            Field.From("Id", "Name"));

        return counties.ToList().Select(CreateSellerDto);
    }

    private static SellerSummaryDto CreateSellerDto(dynamic arg)
    {
        return new SellerSummaryDto
        {
            Id = arg.Id,
            Name = arg.Name
        };
    }
}