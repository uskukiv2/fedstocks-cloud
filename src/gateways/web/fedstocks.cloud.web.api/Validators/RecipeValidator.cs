using fed.cloud.communication.Recipe;
using FluentValidation;

namespace fedstocks.cloud.web.api.Validators;

public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator()
    {
        RuleFor(x => x.RecipeName).NotEmpty();
        RuleFor(x => x.Tags).NotNull();
        RuleFor(x => x.Contents).NotNull().NotEmpty();
        RuleFor(x => x.Ingredients).NotNull().NotEmpty();
    }
}