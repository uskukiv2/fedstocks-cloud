using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.product.application.Models;
using Npgsql;
using RepoDb;

namespace fed.cloud.product.application.Queries.Implementation;

public class CountryQuery : ICountryQuery
{
    private readonly string _connection;

    public CountryQuery(IServiceConfiguration configuration)
    {
        _connection = configuration.Database.Connection;
    }

    public Task<IEnumerable<CountryDto>> GetCountriesAsync(int top = 10)
    {
        throw new NotSupportedException("Get top countries is not supported yet");
    }

    public async Task<IEnumerable<CountyDto>> GetCountyByCountryAsync(Guid countryId)
    {
        await using var conn = new NpgsqlConnection(_connection);

        var counties = await conn.QueryAsync("counties", new {CountryId = countryId},
            Field.From("Number", "Name", "CountryId"));

        return counties.ToList().Select(CreateCountyDto);
    }

    private static CountyDto CreateCountyDto(dynamic county)
    {
        return new CountyDto
        {
            Name = county.Name,
            NumberInCountry = county.Number
        };
    }
}