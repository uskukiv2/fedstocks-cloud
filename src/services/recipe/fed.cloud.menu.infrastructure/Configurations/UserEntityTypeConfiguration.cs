using fed.cloud.menu.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.Property(u => u.Id)
            .HasColumnName(nameof(User.Id).ToLower());
        builder.HasKey(u => u.Id);

        builder.Property(u => u.AuthenticationId)
            .HasColumnName(nameof(User.AuthenticationId).ToLower())
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasColumnName(nameof(User.IsActive).ToLower())
            .IsRequired();

        builder.HasIndex(e => e.AuthenticationId).IsUnique();
    }
}