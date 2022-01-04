using fed.cloud.eventbus.EventBus.Abstraction;
using fed.cloud.product.application.IntegrationEvents.Events;
using fed.cloud.product.application.IntegrationEvents.Handlers;
using fed.cloud.product.infrastructure;
using Microsoft.EntityFrameworkCore;

namespace fed.cloud.product.host.Extensions;

internal static class ApplicationExtensions
{
    internal static WebApplication ConfigureEventBus(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<AddProductPurchasesEvent, AddProductPurchasesEventHandler>();

        return app;
    }
    
    public static void BeforeBuild(this WebApplication app)
    {
        BuildDatabase(app.Services, app.Logger, app.Configuration, app.Environment.IsDevelopment());
    }

    private static void BuildDatabase(IServiceProvider appServices, ILogger appLogger, IConfiguration appConfiguration, bool isDevelopment)
    {
        try
        {
            if (!isDevelopment || !appConfiguration.GetRegenerateDatabase())
            {
                return;
            }

            using var scope = appServices.GetService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductContext>();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
            context.Database.EnsureCreated();
        }
        catch (Exception e)
        {
            appLogger.LogError(e, "Build database failed");
        }
    }
}