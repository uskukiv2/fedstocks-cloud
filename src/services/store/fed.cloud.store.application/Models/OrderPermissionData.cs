namespace fed.cloud.store.application.Models;

public record OrderPermissionData
{
    public PermissionType PermissionNumber { get; }

    public bool IsAllowed { get; }
}