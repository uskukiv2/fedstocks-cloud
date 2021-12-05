using fed.cloud.product.domain.Factories;
using fed.cloud.product.host.Extensions;
using fed.cloud.product.host.Services;
using fed.cloud.product.infrastructure.Factories.Implementation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddLogging();
builder.Services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddServiceConfigurations(config);
builder.Services.AddCustomDbContext(config);
builder.Services.AddIntegrationEvent(config);
builder.Services.AddEventBus(config);
builder.Services.AddMediator();

var app = builder.Build();

// application uses
// Configure the HTTP request pipeline.
app.MapGrpcService<ProductService>();
app.MapGrpcService<SellerService>();
app.MapGrpcService<CountryService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
