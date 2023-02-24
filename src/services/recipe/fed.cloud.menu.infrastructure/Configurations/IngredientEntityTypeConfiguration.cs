using fed.cloud.menu.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class IngredientEntityTypeConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");
        builder.Property(i => i.Id)
            .HasColumnName(nameof(Ingredient.Id).ToLower());
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasColumnName(nameof(Ingredient.Name).ToLower());

        builder.Property(i => i.ProductNumber)
            .IsRequired()
            .HasColumnName(nameof(Ingredient.ProductNumber).ToLower());

        builder.Property(i => i.UnitId)
            .HasColumnName(nameof(Ingredient.UnitId).ToLower());

        builder.HasOne(i => i.Unit)
            .WithMany()
            .HasForeignKey(i => i.UnitId);
    }
}