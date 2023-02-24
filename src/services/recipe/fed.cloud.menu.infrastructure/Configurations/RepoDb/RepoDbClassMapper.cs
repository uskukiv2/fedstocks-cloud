using fed.cloud.menu.data.Entities;
using fed.cloud.menu.data.Models.Read;
using Npgsql.Internal.TypeHandlers;
using RepoDb;

namespace fed.cloud.menu.infrastructure.Configurations.RepoDb;

public class RepoDbClassMapper
{
    public void MapRecipeModels()
    {
        FluentMapper.Entity<RecipeModel>()
            .Table("mview_recipes")
            .Column(u => u.Id, nameof(RecipeModel.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.Name, nameof(RecipeModel.Name).ToLower())
            .Column(u => u.Tags, nameof(RecipeModel.Tags).ToLower())
            .Column(u => u.OwnerId, nameof(RecipeModel.OwnerId).ToLower())
            .Column(u => u.CookingTime, nameof(RecipeModel.CookingTime).ToLower())
            .Column(u => u.Content, nameof(RecipeModel.Content).ToLower());
        
        FluentMapper.Entity<RecipeIngredientModel>()
            .Table("mview_recipeingredients")
            .Column(u => u.Id, nameof(RecipeIngredientModel.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.Quantity, nameof(RecipeIngredientModel.Quantity).ToLower())
            .Column(u => u.RecipeId, nameof(RecipeIngredientModel.RecipeId).ToLower())
            .Column(u => u.ProductNumber, nameof(RecipeIngredientModel.ProductNumber).ToLower())
            .Column(u => u.Rate, nameof(RecipeIngredientModel.Rate).ToLower())
            .Column(u => u.Name, nameof(RecipeIngredientModel.Name).ToLower());
    }

    public void MapUnitEntity()
    {
        FluentMapper.Entity<Unit>()
            .Table("units")
            .Column(u => u.Id, nameof(Unit.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.TypeNumber, nameof(Unit.TypeNumber).ToLower())
            .Column(u => u.Rate, nameof(Unit.Rate).ToLower());
    }

    public void MapIngredientEntity()
    {
        FluentMapper.Entity<Ingredient>()
            .Table("ingredients")
            .Column(u => u.Id, nameof(Ingredient.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.ProductNumber, nameof(Ingredient.ProductNumber).ToLower())
            .Column(u => u.Name, nameof(Ingredient.Name).ToLower())
            .Column(u => u.UnitId, nameof(Ingredient.UnitId).ToLower());
    }
    
    public void MapRecipeEntity()
    {
        FluentMapper.Entity<Recipe>()
            .Table("recipes")
            .Column(u => u.Id, nameof(Recipe.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.Name, nameof(Recipe.Name).ToLower())
            .Column(u => u.Tags, nameof(Recipe.Tags).ToLower())
            .Column(u => u.OwnerId, nameof(Recipe.OwnerId).ToLower())
            .Column(u => u.CookingTime, nameof(Recipe.CookingTime).ToLower())
            .Column(u => u.Content, nameof(Recipe.Content).ToLower());
    }
    
    public void MapRecipeIngredientEntity()
    {
        FluentMapper.Entity<RecipeIngredient>()
            .Table("recipeingredients")
            .Column(u => u.Id, nameof(RecipeIngredient.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.Quantity, nameof(RecipeIngredient.Quantity).ToLower())
            .Column(u => u.RecipeId, nameof(RecipeIngredient.RecipeId).ToLower())
            .Column(u => u.IngredientId, nameof(RecipeIngredient.IngredientId).ToLower());
    }
    
    public void MapUserEntity()
    {
        FluentMapper.Entity<User>()
            .Table("users")
            .Column(u => u.Id, nameof(User.Id).ToLower())
            .Primary(u => u.Id)
            .Identity(u => u.Id)
            .Column(u => u.AuthenticationId, nameof(User.AuthenticationId).ToLower())
            .Column(u => u.IsActive, nameof(User.IsActive).ToLower());
    }
}