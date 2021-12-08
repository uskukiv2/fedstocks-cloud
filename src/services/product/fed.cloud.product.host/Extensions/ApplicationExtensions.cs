using fed.cloud.eventbus.EventBus.Abstraction;
using fed.cloud.product.application.IntegrationEvents.Events;
using fed.cloud.product.application.IntegrationEvents.Handlers;

namespace fed.cloud.product.host.Extensions;

internal static class ApplicationExtensions
{
    internal static WebApplication ConfigureEventBus(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<AddProductPurchasesEvent, AddProductPurchasesEventHandler>();

        return app;
    }
}