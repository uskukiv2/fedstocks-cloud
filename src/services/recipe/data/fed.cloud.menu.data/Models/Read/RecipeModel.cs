using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Models.Read;

#nullable disable

public class RecipeModel : ReadModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public Guid OwnerId { get; set; }
    
    public string Tags { get; set; }
    
    public TimeSpan CookingTime { get; set; }
    
    public string Content { get; set; }
    
    public virtual IEnumerable<RecipeIngredientModel> Ingredients { get; set; }
}