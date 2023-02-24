using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Models.Read;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fed.cloud.menu.infrastructure.Configurations;

public class RecipeModelEntityTypeConfiguration : IEntityTypeConfiguration<RecipeModel>
{
    public void Configure(EntityTypeBuilder<RecipeModel> builder)
    {
        builder.HasNoKey();
        builder.ToView("mview_recipes");

        builder.Property(r => r.Id).HasColumnName(nameof(RecipeModel.Id).ToLower());
        builder.Property(r => r.Name).HasColumnName(nameof(RecipeModel.Name).ToLower());
        builder.Property(r => r.OwnerId).HasColumnName(nameof(RecipeModel.OwnerId).ToLower());
        builder.Property(r => r.Tags).HasColumnName(nameof(RecipeModel.Tags).ToLower());
        builder.Property(r => r.CookingTime).HasColumnName(nameof(RecipeModel.CookingTime).ToLower());
        builder.Property(r => r.Content).HasColumnName(nameof(RecipeModel.Content).ToLower());
    }
}

public class RecipeIngredientModelEntityTypeConfiguration : IEntityTypeConfiguration<RecipeIngredientModel>
{
    public void Configure(EntityTypeBuilder<RecipeIngredientModel> builder)
    {
        builder.HasNoKey();
        builder.ToView("mview_recipeingredients");

        builder.Property(r => r.Id).HasColumnName(nameof(RecipeIngredientModel.Id).ToLower());
        builder.Property(r => r.Name).HasColumnName(nameof(RecipeIngredientModel.Name).ToLower());
        builder.Property(r => r.ProductNumber).HasColumnName(nameof(RecipeIngredientModel.ProductNumber).ToLower());
        builder.Property(r => r.Quantity).HasColumnName(nameof(RecipeIngredientModel.Quantity).ToLower());
        builder.Property(r => r.Rate).HasColumnName(nameof(RecipeIngredientModel.Rate).ToLower());
        builder.Property(r => r.RecipeId).HasColumnName(nameof(RecipeIngredientModel.RecipeId).ToLower());
    }
}