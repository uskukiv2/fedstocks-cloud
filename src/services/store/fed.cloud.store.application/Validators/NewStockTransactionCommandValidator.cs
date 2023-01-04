using fed.cloud.store.application.Commands;
using FluentValidation;
using System;

namespace fed.cloud.store.application.Validators;

public class NewStockTransactionCommandValidator : AbstractValidator<NewStockTransactionCommand>
{
    public NewStockTransactionCommandValidator()
    {
        RuleFor(x => x.StockId).NotNull().NotEqual(Guid.Empty).WithMessage("empty stock id is not allowed");
        RuleFor(x => x.Items).NotNull().NotEmpty().WithMessage("Cannot continue with empty stock items");
        RuleForEach(x => x.Items).ChildRules(x => x.RuleFor(x => x.CategoryId).GreaterThan(0));
        RuleForEach(x => x.Items).ChildRules(x => x.RuleFor(x => x.UnitId).GreaterThan(0));
        RuleForEach(x => x.Items).ChildRules(x => x.RuleFor(x => x.ProductName).NotEmpty());
        RuleForEach(x => x.Items).ChildRules(x => x.RuleFor(x => x.ProductNumber).GreaterThan(0));
    }
}