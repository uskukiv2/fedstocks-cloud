using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Repository;
using Microsoft.EntityFrameworkCore;

namespace fed.cloud.menu.infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly MenuContext _context;

    public AuditLogRepository(MenuContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;
    
    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string targetBusinessEntityName)
    {
        return await _context.AuditLogs.Where(x => x.EntityName == targetBusinessEntityName).ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId)
    {
        return await _context.AuditLogs.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task AddAsync(AuditLog entity)
    {
        await _context.AuditLogs.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<AuditLog> entity)
    {
        await _context.AuditLogs.AddRangeAsync(entity);
    }

    public async Task DeleteRangeByDatesAsync(DateTime startAt, DateTime endAt)
    {
        await _context.Database.ExecuteSqlRawAsync(
            $@"DELETE FROM ""auditlogs"" al
                WHERE al.ModifiedAt BETWEEN '{startAt.ToString(GetDefaultTimeFormat())}'
                    AND '{endAt.ToString(GetDefaultTimeFormat())}'");
    }

    private string GetDefaultTimeFormat()
    {
        return "s";
    }
}