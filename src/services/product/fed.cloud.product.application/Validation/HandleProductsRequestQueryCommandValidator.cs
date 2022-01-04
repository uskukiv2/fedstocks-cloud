using fed.cloud.product.application.Commands;
using FluentValidation;

namespace fed.cloud.product.application.Validation;

public class HandleProductsRequestQueryCommandValidator : AbstractValidator<HandleProductsRequestQueryCommand>
{
    public HandleProductsRequestQueryCommandValidator()
    {
        RuleFor(x => x.Query).NotNull().NotEmpty();
    }
}