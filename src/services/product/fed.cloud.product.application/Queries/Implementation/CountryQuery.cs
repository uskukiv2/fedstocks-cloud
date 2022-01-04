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

    public async Task<CountryDto> GetCountryByIdAsync(Guid countryId)
    {
        await using var conn = new NpgsqlConnection(_connection);
        var country = (await conn.QueryAsync("product.countries", new { Id = countryId })).FirstOrDefault();
        var counties = await conn.QueryAsync("product.counties", new { CountryId = country.Id });

        return MapToCountryDto(country, counties);
    }

    private static CountryDto MapToCountryDto(dynamic country, IEnumerable<dynamic> counties)
    {
        return new CountryDto
        {
            Id = country.Id,
            Name = country.Name,
            Number = int.Parse(country.GlobalId),
            CountyDtos = counties.Select(CreateCountyDto).ToArray()
        };
    }
    
    private static CountyDto CreateCountyDto(dynamic county)
    {
        return new CountyDto
        {
            Id = county.Id,
            Name = county.Name,
            Number = county.Number
        };
    }
}