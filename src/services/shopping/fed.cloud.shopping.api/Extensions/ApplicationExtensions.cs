using fed.cloud.eventbus.EventBus.Abstraction;

namespace fed.cloud.product.host.Extensions;

internal static class ApplicationExtensions
{
    internal static WebApplication ConfigureEventBus(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();

        return app;
    }
}