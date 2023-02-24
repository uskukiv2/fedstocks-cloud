using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Models;
using fed.cloud.menu.data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace fed.cloud.menu.infrastructure.Services;

public interface IAuditManager
{
    Task WriteAuditAsync(Guid callerId, IEnumerable<EntityEntry> entries);
}

public class AuditManager : IAuditManager
{
    private readonly IAuditLogRepository _logRepository;

    public AuditManager(IAuditLogRepository logRepository)
    {
        _logRepository = logRepository;
    }


    public async Task WriteAuditAsync(Guid callerId, IEnumerable<EntityEntry> entries)
    {
        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            var auditLog = CreateAuditLog(entry);
            if (auditLog == null)
            {
                continue;
            }
            auditLog.UserId = callerId;
            auditLogs.Add(auditLog);
        }

        await _logRepository.AddRangeAsync(auditLogs);
    }

    private AuditLog? CreateAuditLog(EntityEntry entry)
    {
        if (entry.Entity is AuditLog or User
            || entry.State is EntityState.Detached or EntityState.Unchanged)
        {
            return null;
        }

        var auditLog = new AuditLog
        {
            EntityName = entry.Metadata.Name,
        };

        var affectedColumns = new List<string>();
        var oldValues = new Dictionary<string, object>();
        var newValues = new Dictionary<string, object>();
        KeyValuePair<string, object>? primaryKey = null;

        foreach (var entryProperty in entry.Properties)
        {
            var name = entryProperty.Metadata.Name;
            if (entryProperty.Metadata.IsPrimaryKey())
            {
                primaryKey = new KeyValuePair<string, object>(name, entryProperty.CurrentValue);
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    newValues.Add(name, entryProperty.CurrentValue);
                    break;
                case EntityState.Deleted:
                    oldValues.Add(name, entryProperty.OriginalValue);
                    break;
                case EntityState.Modified:
                    if (entryProperty.IsModified)
                    {
                        affectedColumns.Add(name);
                        oldValues.Add(name, entryProperty.OriginalValue);
                        newValues.Add(name, entryProperty.CurrentValue);
                    }

                    break;
            }

            // we don't want to store whole changes of new row
            if (entry.State == EntityState.Added)
            {
                break;
            }
        }

        auditLog.AffectedColumns = string.Join(';', affectedColumns);
        auditLog.PrimaryKey = primaryKey.ToString()!;
        auditLog.OldValues = oldValues.Count > 0 ? JsonConvert.SerializeObject(oldValues) : string.Empty;
        auditLog.NewValues = newValues.Count > 0 ? JsonConvert.SerializeObject(newValues) : string.Empty;

        return auditLog;
    }
}