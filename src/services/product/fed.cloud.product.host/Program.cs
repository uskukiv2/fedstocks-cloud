using Autofac;
using Autofac.Extensions.DependencyInjection;
using fed.cloud.common.Helpers;
using fed.cloud.product.application.Validation;
using fed.cloud.product.host.Extensions;
using fed.cloud.product.host.Infrastructure;
using fed.cloud.product.host.Services;
using FluentValidation;
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

builder.WebHost.UseDefaultServiceProvider(o => o.ValidateScopes = false);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var config = builder.Configuration;

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
RepoDb.PostgreSqlBootstrap.Initialize();
builder.Services.AddServiceConfigurations(config);
builder.Services.AddCustomDbContext(config);
builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
});
builder.Services.AddValidatorsFromAssembly(typeof(HandleProductsRequestQueryCommandValidator).Assembly, includeInternalTypes: true);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddGrpcReflection();
}

// last
builder.Services.AddIntegrationEvent(config);
builder.Services.AddEventBus(config);

builder.Host.ConfigureContainer<ContainerBuilder>(b =>
{
    b.RegisterModule(new MediatorModule());
    b.RegisterModule(new ApplicationModule());
});

var app = builder.Build();

app.BeforeBuild();

// application uses
// Configure the HTTP request pipeline.

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<ProductService>();
    endpoint.MapGrpcService<SellerService>();
    endpoint.MapGrpcService<CountryService>();
    endpoint.MapGet("/",
        async context =>
            await context.Response.WriteAsync(
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client"));

    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
});

app.ConfigureEventBus();

app.Run();
