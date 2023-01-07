using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace gen.fed.application.Models.Recipes;

public class RecipeDto
{
    public int Id { get; set; }

    public string ImgUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(100), MinLength(10)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(2, 20), MinLength(2)]
    public ObservableCollection<RecipeIngredientDto> Ingredients { get; set; } = new();

    [Required, Range(2, 10), MinLength(2)]
    public ObservableCollection<RecipePreparationDto> Preparations { get; set; } = new();
}

public class RecipeIngredientDto
{
    public RecipeIngredientDto(int id)
    {
        Id = id;
    }

    public int Id { get; }

    [Required, StringLength(50), MinLength(5)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(0.1, 10.0)]
    public double Quantity { get; set; } = 0.0;

    [Required, StringLength(20)]
    public string UnitName { get; set; } = string.Empty;
}

public class RecipePreparationDto
{
    public RecipePreparationDto(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public int NumberOfOrder { get; set; }

    [Required, MinLength(5)]
    public string Content { get; set; } = string.Empty;
}

public enum IngredientUnitType
{
    Gram = 1,
    Peace = 2,
    Milliliters = 3
}