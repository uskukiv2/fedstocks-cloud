﻿using fed.cloud.communication.Shopper;
using FluentValidation;

namespace fedstocks.cloud.web.api.Validators;

public class BaseShoppingListValidator<T> : AbstractValidator<T> where T : BaseShoppingList
{
    public BaseShoppingListValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Seller).NotNull();
        RuleForEach(x => x.Lines).Cascade(CascadeMode.Continue).ChildRules(x =>
        {
            x.RuleFor(y => y.Quantity).GreaterThanOrEqualTo(1);
            x.RuleFor(y => y.UnitPrice).GreaterThanOrEqualTo(decimal.One);
            x.RuleFor(y => y.ProductBrand).NotEmpty();
            x.RuleFor(y => y.ProductName).NotEmpty();
            x.RuleFor(y => y.ProductNumber).GreaterThanOrEqualTo(1);
            x.RuleFor(y => y.Unit).NotNull();
        });
    }
}

public class NewShoppingListValidator : BaseShoppingListValidator<NewShoppingList>
{
    public NewShoppingListValidator()
    {
        RuleFor(x => x).NotNull();
    }
}

public class CompletedShoppingListValidator : BaseShoppingListValidator<CompletedShoppingList>
{
    public CompletedShoppingListValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id).GreaterThanOrEqualTo(0);
    }
}
