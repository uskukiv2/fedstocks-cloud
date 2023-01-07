using gen.fed.application.Services;
using gen.fed.web.domain.Abstract;
using gen.fed.web.domain.Factories;
using gen.fed.web.domain.Repositories;
using gen.fed.web.infrastructure;
using gen.fed.web.infrastructure.Factories;
using gen.fed.web.infrastructure.Repositories;
using gen.fedstocks.web.server.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using Npgsql;

namespace gen.fedstocks.web.server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFed(this IServiceCollection services)
    {
        RegisterServices(services);

        RegisterRepositories(services);

        RegisterFactories(services);

        RegisterRoutingManager(services);

        RegisterMudBlazor(services);

        RegisterIconManager(services);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddEntityFrameworkNpgsql()
            .AddDbContext<ServiceContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("default"));
            });
        service.AddScoped<IUnitOfWork<NpgsqlConnection>>(sp => sp.GetRequiredService<ServiceContext>());
        return service;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void RegisterFactories(IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IPageManager, PageManager>();
        services.AddSingleton<ITitleService, TitleService>();

        services.AddScoped<ITopbarItemsService, TopbarItemsService>();
    }

    private static void RegisterIconManager(IServiceCollection service)
    {
        service.AddSingleton<CommandIconManager>();
    }

    private static void RegisterRoutingManager(IServiceCollection services)
    {
        services.AddSingleton<IRoutingManager>(sp => new RoutingManager(RouteExtensions.GetRoutes()));
    }

    private static void RegisterMudBlazor(IServiceCollection services)
    {
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
    }
}