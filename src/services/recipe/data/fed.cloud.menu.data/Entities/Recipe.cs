using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Entities;

public class Recipe : IEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Tags { get; set; }
    
    public TimeSpan CookingTime { get; set; }
    
    public string Content { get; set; }
    
    public Guid OwnerId { get; set; }
    
    public virtual User Owner { get; set; }
    
    public virtual IEnumerable<RecipeIngredient> Ingredients { get; set; }
}