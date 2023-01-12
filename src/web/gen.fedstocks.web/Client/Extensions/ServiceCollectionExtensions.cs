using gen.fedstocks.web.Client.Services;
using MudBlazor;
using MudBlazor.Services;

namespace gen.fedstocks.web.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClient(this IServiceCollection services)
    {
        RegisterServices(services);

        RegisterMudBlazor(services);

        RegisterIconManager(services);

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ITopbarItemsService, TopbarItemsService>();
    }

    private static void RegisterIconManager(IServiceCollection service)
    {
        service.AddSingleton<CommandIconManager>();
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