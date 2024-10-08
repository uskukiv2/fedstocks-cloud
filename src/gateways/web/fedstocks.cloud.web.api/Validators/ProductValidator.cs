using fed.cloud.communication.Product;
using FluentValidation;

namespace fedstocks.cloud.web.api.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Number).GreaterThan(0);
    }
}