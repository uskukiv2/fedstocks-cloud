using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace gen.fedstocks.web.Client.Application.Models.Recipes;

public class RecipeDto
{
    public int Id { get; set; }
    
    public Guid RecipeId { get; set; }

    [Required]
    [StringLength(100), MinLength(10)]
    public string Name { get; set; } = string.Empty;

    public bool IsNew => Id < 0 && RecipeId == Guid.Empty;

    [Range(0, 5), MinLength(0), MaxLength(5)]
    public ObservableCollection<string> Tags { get; set; } = new();

    [Required]
    public TimeSpan CookingTime { get; set; } = TimeSpan.Zero;

    [Required, Range(2, 20), MinLength(2)]
    public ObservableCollection<RecipeIngredientDto> Ingredients { get; set; } = new();

    [Required, Range(2, 10), MinLength(2)]
    public ObservableCollection<RecipePreparationDto> Preparations { get; set; } = new();
}