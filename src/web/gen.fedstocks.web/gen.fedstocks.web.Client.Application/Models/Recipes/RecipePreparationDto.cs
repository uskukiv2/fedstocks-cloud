using System.ComponentModel.DataAnnotations;

namespace gen.fedstocks.web.Client.Application.Models.Recipes;

public class RecipePreparationDto
{
    public int Id { get; set; }

    [Required, MinLength(5)]
    public string Content { get; set; } = string.Empty;
}