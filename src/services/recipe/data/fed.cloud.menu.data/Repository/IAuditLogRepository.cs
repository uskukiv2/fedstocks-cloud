using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;

namespace fed.cloud.menu.data.Repository;

public interface IAuditLogRepository : IRepository
{
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string targetBusinessEntityName);

    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);

    Task AddAsync(AuditLog entity);

    Task AddRangeAsync(IEnumerable<AuditLog> entity);

    Task DeleteRangeByDatesAsync(DateTime startAt, DateTime endAt);
}