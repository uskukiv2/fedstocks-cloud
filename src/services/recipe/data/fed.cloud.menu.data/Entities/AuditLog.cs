using fed.cloud.menu.data.Abstract;
using fed.cloud.menu.data.Models;

namespace fed.cloud.menu.data.Entities;

public class AuditLog : IEntity
{
    public Guid Id { get; set; }

    public string EntityName { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string? OldValues { get; set; }
    
    public string? NewValues { get; set; }
    
    public string? AffectedColumns { get; set; }

    public string PrimaryKey { get; set; } = string.Empty;
    
    public ChangeType Change { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    public virtual User User { get; set; }
}