using fed.cloud.menu.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class AuditLogsEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("auditlogs");

        builder.Property(a => a.Id)
            .HasColumnName(nameof(AuditLog.Id).ToLower());
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityName)
            .HasColumnName(nameof(AuditLog.EntityName).ToLower())
            .IsRequired();

        builder.Property(a => a.Description)
            .HasColumnName(nameof(AuditLog.Description).ToLower())
            .IsRequired();

        builder.Property(a => a.PrimaryKey)
            .HasColumnName(nameof(AuditLog.PrimaryKey).ToLower())
            .IsRequired();
        
        builder.Property(a => a.AffectedColumns)
            .HasColumnName(nameof(AuditLog.AffectedColumns).ToLower());

        builder.Property(a => a.OldValues)
            .HasColumnName(nameof(AuditLog.OldValues).ToLower());

        builder.Property(a => a.NewValues)
            .HasColumnName(nameof(AuditLog.NewValues).ToLower());

        builder.Property(a => a.Change)
            .HasColumnName(nameof(AuditLog.Change).ToLower())
            .IsRequired();

        builder.Property(a => a.ModifiedDate)
            .HasColumnName(nameof(AuditLog.ModifiedDate).ToLower())
            .IsRequired();

        builder.Property(a => a.UserId)
            .HasColumnName(nameof(AuditLog.UserId).ToLower());

        builder.HasOne(a => a.User)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction)
            .HasForeignKey(a => a.UserId);
    }
}