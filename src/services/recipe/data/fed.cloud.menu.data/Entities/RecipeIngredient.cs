using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Entities;

public class RecipeIngredient : IEntity
{
    public Guid Id { get; set; }
    
    public Guid RecipeId { get; set; }
    
    public Guid IngredientId { get; set; }
    
    public double Quantity { get; set; }
    
    public virtual Ingredient Ingredient { get; set; }
}