using gen.common.Extensions;
using gen.fed.ui.Abstract;
using gen.fed.ui.Factories;
using gen.fed.ui.Factories.Impl;
using gen.fed.ui.Services;
using gen.fed.ui.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace gen.fed.ui.Extensions;

public static class ServiceUiExtensions
{
    public static IServiceCollection AddUI(this IServiceCollection service, IReadOnlyCollection<Type> allTypes)
    {
        RegisterReactive(service);

        RegisterServices(service, allTypes);

        RegisterViewModels(service, allTypes);

        RegisterAuthentication(service, allTypes);

        return service;
    }

    private static void RegisterReactive(IServiceCollection service)
    {
        service.UseMicrosoftDependencyResolver();

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();

        service.AddSingleton<IMessageBus, MessageBus>();

        service.AddSingleton<NavigationState>();

        service.AddSingleton<ICurrentViewModelObservableContainer>(sp => sp.GetRequiredService<NavigationState>());
    }

    private static void RegisterServices(IServiceCollection service, IReadOnlyCollection<Type> allTypes)
    {
        RegisterCommonServices(service, allTypes);

        RegisterFactories(service);
    }

    private static void RegisterFactories(IServiceCollection service)
    {
        service.AddSingleton<IViewModelFactory, ViewModelFactory>();
    }


    private static void RegisterCommonServices(IServiceCollection service, IReadOnlyCollection<Type> allTypes)
    {
        var baseType = typeof(IService);
        var types = allTypes.DescendantOf(baseType).Where(x => x.IsInterface);
        foreach (var type in types)
        {
            var classType =
                allTypes.SingleOrDefault(x => !x.IsInterface && !x.IsAbstract && x.GetInterface(type.Name) != null)!;

            service.AddSingleton(type, classType);
        }
    }

    private static void RegisterViewModels(IServiceCollection service, IReadOnlyCollection<Type> allTypes)
    {
        var baseType = typeof(BaseViewModel);

        var vmTypes = allTypes.DescendantOf(baseType).OnlyInstantiableClasses();

        foreach (var type in vmTypes)
        {
            service.AddTransient(type);
        }
    }

    private static void RegisterAuthentication(IServiceCollection service, IReadOnlyCollection<Type> allTypes)
    {
        RegisterAuthenticationProvider(service, allTypes);
        RegisterAuthenticationService(service);
    }

    private static void RegisterAuthenticationService(IServiceCollection service)
    {
        service.AddScoped<IApplicationService, ApplicationService>();
    }

    private static void RegisterAuthenticationProvider(IServiceCollection service, IReadOnlyCollection<Type> allTypes)
    {
        var baseType = typeof(IAuthenticationProvider);
        var type = allTypes.DescendantOf(baseType).OnlyInstantiableClasses().SingleOrDefault();
        if (type == null)
        {
            throw new NullReferenceException(baseType.Name);
        }

        service.AddScoped(baseType, type);
    }
}