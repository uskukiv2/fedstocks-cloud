using fed.cloud.shopping.api.Extensions;
using fed.cloud.shopping.api.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddServiceConfigurations(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddIntegrationEvent(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddDbOperations();

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ShoppingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
