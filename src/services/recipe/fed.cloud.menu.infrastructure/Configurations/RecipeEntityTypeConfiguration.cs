using fed.cloud.menu.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class RecipeEntityTypeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("recipes");
        builder.Property(i => i.Id)
            .HasColumnName(nameof(Recipe.Id).ToLower());
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasColumnName(nameof(Recipe.Name).ToLower());

        builder.Property(r => r.Tags)
            .IsRequired()
            .HasColumnName(nameof(Recipe.Tags).ToLower());

        builder.Property(r => r.CookingTime)
            .HasColumnName(nameof(Recipe.CookingTime).ToLower());

        builder.Property(r => r.Content)
            .IsRequired()
            .HasColumnName(nameof(Recipe.Content).ToLower());

        builder.Property(r => r.OwnerId)
            .IsRequired()
            .HasColumnName(nameof(Recipe.OwnerId).ToLower());

        builder.HasMany(r => r.Ingredients)
            .WithOne()
            .IsRequired()
            .HasForeignKey(r => r.RecipeId);

        builder.Navigation(r => r.Ingredients)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.HasOne(r => r.Owner)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}