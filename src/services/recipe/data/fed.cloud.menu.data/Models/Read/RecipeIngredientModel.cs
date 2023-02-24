using fed.cloud.menu.data.Abstract;

namespace fed.cloud.menu.data.Models.Read;

#nullable disable

public class RecipeIngredientModel : ReadModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int ProductNumber { get; set; }
    
    public double Quantity { get; set; }
    
    public double Rate { get; set; }

    public Guid RecipeId { get; set; }
}