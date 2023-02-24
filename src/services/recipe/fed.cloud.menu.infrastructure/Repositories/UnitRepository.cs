using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace fed.cloud.menu.infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly MenuContext _context;

    public UnitRepository(MenuContext context)
    {
        _context = context;
    }
    
    public IUnitOfWork UnitOfWork => _context;


    public Task<Unit> GetUnitByIdAsync(int id)
    {
        return _context.UnitTypes.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Unit> AddAsync(Unit unitType, CancellationToken ct)
    {
        await _context.UnitTypes.AddAsync(unitType, ct);
        _context.Entry(unitType).State = EntityState.Added;

        return unitType;
    }

    public Task<Unit> UpdateAsync(Unit unitType, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            _context.UnitTypes.Update(unitType);
            _context.Entry(unitType).State = EntityState.Modified;
            return Task.FromResult(unitType);
        }, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var unitType = await GetUnitByIdAsync(id);
        _context.UnitTypes.Remove(unitType);
        _context.Entry(unitType).State = EntityState.Deleted;
    }
}