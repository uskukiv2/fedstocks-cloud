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

public class SellerCompanyRepository : ISellerCompanyRepository
{
    private readonly ProductContext _context;

    public SellerCompanyRepository(ProductContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<SellerCompany> GetAsync(Guid id)
    {
        return await _context.Companies.AsNoTracking()
            .Include(x => x.Country)
            .Include(x => x.County)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void Add(SellerCompany entity, CancellationToken token = default)
    {
        Task.Factory.StartNew(async () =>
        {
            await _context.Companies.AddAsync(entity, token);
            _context.Entry(entity).State = EntityState.Added;
        }, token);
    }

    public void Update(SellerCompany entity, CancellationToken token = default)
    {
        _context.Companies.Update(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<bool> IsCompanyExistsAsync(Guid sellerId)
    {
        return await _context.Companies.AnyAsync(x => x.Id == sellerId);
    }

    public async Task<IEnumerable<SellerCompany>> TTSearchAsync(string query, Guid countryId, CancellationToken token)
    {
        return await _context.Companies.Where(x => x.SearchVector.Matches(query) && x.CountryId == countryId)
            .ToListAsync(token);
    }

    public async Task<IEnumerable<SellerCompany>> TTSearchAsync(string query, Guid countryId, Guid countyId, CancellationToken token)
    { 
        return await _context.Companies
            .Where(x => x.SearchVector.Matches(query) && x.CountryId == countryId && x.CountyId == countyId)
            .ToListAsync(token);
    }
}