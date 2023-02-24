using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;

namespace fed.cloud.menu.data.Repository;

public interface IUnitRepository : IRepository
{
    Task<Unit> GetUnitByIdAsync(int id);

    Task<Unit> AddAsync(Unit unitType, CancellationToken ct);
    Task<Unit> UpdateAsync(Unit unitType, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}