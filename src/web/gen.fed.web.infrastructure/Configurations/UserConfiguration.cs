using gen.fed.web.domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gen.fed.web.infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.AuthenticationId)
            .IsRequired()
            .HasColumnName(nameof(User.AuthenticationId));

        builder.Property(u => u.RoleId)
            .IsRequired()
            .HasColumnName(nameof(User.RoleId));

        builder.Property(u => u.InternalId)
            .IsRequired()
            .HasColumnName(nameof(User.InternalId));
    }
}