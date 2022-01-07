using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fed.cloud.common.Infrastructure;
using fed.cloud.product.domain.Entities;
using fed.cloud.product.domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace fed.cloud.product.infrastructure.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly ProductContext _context;

    public CountryRepository(ProductContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Country> GetAsync(Guid id)
    {
        return await _context.Countries.AsNoTracking()
            .Include(x => x.Counties)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Add(Country entity, CancellationToken token = default)
    {
        Task.Factory.StartNew(async () =>
        {
            await _context.Countries.AddAsync(entity, token);
            _context.Entry(entity).State = EntityState.Added;
        }, token);
    }

    public void Update(Country entity, CancellationToken token = default)
    {
        _context.Countries.Update(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<IEnumerable<Country>> TTSearchAsync(string query, CancellationToken token)
    {
        return await _context.Countries.Where(x => x.SearchVector.Matches(query)).ToListAsync(token);
    }

    public async Task<County> GetCountyOfCountryAsync(Guid country, int countyNumber)
    {
        return (await _context.Countries.AsNoTracking().Include(x => x.Counties)
                .FirstOrDefaultAsync(x => x.Id == country))?.Counties
            .FirstOrDefault(x => x.NumberInCountry == countyNumber)!;
    }
}