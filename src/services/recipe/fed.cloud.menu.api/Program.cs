using System.ComponentModel;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using fed.cloud.menu.api;
using fed.cloud.menu.api.Extensions;
using fed.cloud.menu.api.Interceptors;
using fed.cloud.menu.api.Services;
using fed.cloud.menu.data.Entities;
using fed.cloud.menu.infrastructure;
using fed.cloud.recipe.application;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseDefaultServiceProvider(o => o.ValidateScopes = false);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.

builder.Services.AddLogging(cfg =>
{
    cfg.SetMinimumLevel(LogLevel.Trace);
    cfg.AddDebug();
    cfg.AddConsole();
});

builder.Services.AddSecurity();
builder.Services.AddGrpc(e =>
{
    e.Interceptors.Add<ServerLoggingInterceptor>();
});
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMediator();
builder.Services.AddMapper();

builder.Host.ConfigureContainer<ContainerBuilder>(b =>
{
    b.RegisterModule<ApplicationModule>();
    b.RegisterModule<InfrastructureModule>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSecurity();
app.MapGrpcService<RecipeService>();

app.Run();