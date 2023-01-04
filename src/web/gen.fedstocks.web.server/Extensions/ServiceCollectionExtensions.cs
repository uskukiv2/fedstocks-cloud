using gen.fed.ui.Services;
using gen.fedstocks.web.server.Services;
using MudBlazor;
using MudBlazor.Services;

namespace gen.fedstocks.web.server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFed(this IServiceCollection services)
    {
        AddUIRoutingManager(services);
        AddMud(services);
        AddIconManager(services);
        return services;
    }

    private static void AddIconManager(IServiceCollection service)
    {
        service.AddSingleton<CommandIconManager>();
    }

    private static void AddUIRoutingManager(IServiceCollection services)
    {
        services.AddSingleton<IRoutingManager>(sp => new RoutingManager(RouteExtensions.GetRoutes()));
    }

    private static void AddMud(IServiceCollection services)
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