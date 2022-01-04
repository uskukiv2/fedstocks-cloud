using FluentValidation;

namespace fedstocks.cloud.web.api.Infrastructure.Factories;

public class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IValidator<T> GetValidator<T>()
    {
        return _serviceProvider.GetRequiredService<IValidator<T>>();
    }

    [Obsolete($"Use {nameof(GetValidator)}", true)]
    public IValidator GetValidator(Type type)
    {
        throw new NotImplementedException();
    }
}
