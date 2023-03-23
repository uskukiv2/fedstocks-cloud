using fed.cloud.common.Helpers;
using fed.cloud.shopping.api.Extensions;
using fed.cloud.shopping.api.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
var cert = CertHelper.GetCertificate(builder.Configuration.GetServicesCertPath(), builder.Configuration.GetServiceKeyCertPath());
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Set TLS 1.3 protocol for encrypted 
    serverOptions.ConfigureHttpsDefaults(listenOptions =>
    {
        listenOptions.SslProtocols = SslProtocols.Tls13;
    });

    serverOptions.Listen(IPAddress.Loopback, builder.Configuration.GetPort(), listenOptions =>
    {
        listenOptions.UseHttps(cert);

        // For logging decrypted HTTP traffic
        listenOptions.UseConnectionLogging();
    });
});
// Add services to the container.
builder.Services.AddLogging(x =>
{
    x.SetMinimumLevel(builder.Configuration.GetDefaultLogLevel());
    if (!builder.Environment.IsDevelopment())
    {
        //TODO: FED-129 Implement remote logging service
    }
});

builder.Services.AddServiceConfigurations(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddIntegrationEvent(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddDbOperations();

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
});

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
app.UseEndpoints(e =>
{
    e.MapGrpcService<ShoppingService>();
    e.MapGet("/",
        async context =>
            await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client"));
});

app.Run();
