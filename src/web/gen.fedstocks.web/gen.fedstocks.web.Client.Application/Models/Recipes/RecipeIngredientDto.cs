using System.ComponentModel.DataAnnotations;

namespace gen.fedstocks.web.Client.Application.Models.Recipes;

public class RecipeIngredientDto
{
    public RecipeIngredientDto(int id, Guid recipeIngredientId)
    {
        Id = id;
        RecipeIngredientId = recipeIngredientId;
    }

    public int Id { get; }
    
    public Guid RecipeIngredientId { get; }

    [Required, StringLength(50), MinLength(5)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0.1, 10.0)]
    public double Quantity { get; set; }

    [Required, StringLength(20)]
    public string UnitName { get; set; } = string.Empty;
}