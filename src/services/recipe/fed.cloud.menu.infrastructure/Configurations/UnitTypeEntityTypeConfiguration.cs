using fed.cloud.menu.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class UnitTypeEntityTypeConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units");
        builder.Property(i => i.Id)
            .HasColumnName(nameof(Unit.Id).ToLower());
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.TypeNumber)
            .IsRequired()
            .HasColumnName(nameof(Unit.TypeNumber).ToLower());

        builder.Property(u => u.Rate)
            .IsRequired()
            .HasColumnName(nameof(Unit.Rate).ToLower());
    }
}