using fed.cloud.communication.Country;
using FluentValidation;

namespace fedstocks.cloud.web.api.Validators;

public class CountryValidator : AbstractValidator<Country>
{
    public CountryValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Counties).NotEmpty();
        RuleForEach(x => x.Counties).Cascade(CascadeMode.Continue).ChildRules(x =>
        {
            x.RuleFor(z => z.Id).NotNull().NotEqual(Guid.Empty);
            x.RuleFor(z => z.Name).NotEmpty();
            x.RuleFor(z => z.Number).NotNull().GreaterThan(0);
        });
    }
}