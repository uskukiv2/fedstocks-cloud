using fed.cloud.menu.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class RecipeIngredientEntityTypeConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipeingredients");
        builder.Property(i => i.Id)
            .HasColumnName(nameof(RecipeIngredient.Id).ToLower());
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Quantity)
            .IsRequired()
            .HasColumnName(nameof(RecipeIngredient.Quantity).ToLower());

        builder.Property(ri => ri.RecipeId)
            .IsRequired()
            .HasColumnName(nameof(RecipeIngredient.RecipeId).ToLower());
        
        builder.Property(ri => ri.IngredientId)
            .IsRequired()
            .HasColumnName(nameof(RecipeIngredient.IngredientId).ToLower());

        builder.HasOne(ri => ri.Ingredient)
            .WithMany()
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}