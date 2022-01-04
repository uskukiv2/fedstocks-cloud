using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using fed.cloud.common.Helpers;
using fed.cloud.shopping.api.Extensions;
using fed.cloud.shopping.api.Grpc;
using fed.cloud.shopping.api.Services;
using Microsoft.AspNetCore.Authentication.Certificate;

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
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(o =>
    {
        o.AllowedCertificateTypes =
            builder.Environment.IsDevelopment() ? CertificateTypes.SelfSigned : CertificateTypes.Chained;

        o.RevocationMode = builder.Environment.IsDevelopment() ? X509RevocationMode.NoCheck : X509RevocationMode.Online;

        o.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                var claims = new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier, 
                        context.ClientCertificate.Subject,
                        ClaimValueTypes.String, 
                        context.Options.ClaimsIssuer),
                    new Claim(ClaimTypes.Name,
                        context.ClientCertificate.Subject,
                        ClaimValueTypes.String, 
                        context.Options.ClaimsIssuer)
                };

                context.Principal = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, context.Scheme.Name));
                context.Success();

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddServiceConfigurations(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddIntegrationEvent(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddDbOperations();

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
    o.Interceptors.Add<GlobalLoggingInterceptor>();
});

var app = builder.Build();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

// Configure the HTTP request pipeline.
app.UseEndpoints(e =>
{
    e.MapGrpcService<ShoppingService>();
    e.MapGet("/",
        async context =>
            await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client"));
});

app.Run();
