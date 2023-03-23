using fedstocks.cloud.web.api.Grpc;
using fedstocks.cloud.web.api.Services;
using fedstocks.cloud.web.api.Services.Implementation;
using System.Security.Cryptography.X509Certificates;
using fedstocks.cloud.web.api.Infrastructure.Middlewares;
using fedstocks.cloud.web.api.Mappings;
using fedstocks.cloud.web.api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Keycloak.AuthServices.Authentication;
using Mapster;
using MapsterMapper;
using RemoteCountry = fed.cloud.product.host.Protos.Country;
using RemoteProduct = fed.cloud.product.host.Protos.Product;
using RemoteSeller = fed.cloud.product.host.Protos.Seller;
using RemoteShopping = fed.cloud.shopping.api.Protos.Shopping;
using RemoteRecipe = fed.cloud.menu.api.Protos.Recipe;
using Autofac.Core;
using fedstocks.cloud.web.api.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace fedstocks.cloud.web.api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection service)
    {
        service.AddFluentValidationAutoValidation();
        service.AddFluentValidationClientsideAdapters();
        service.AddValidatorsFromAssemblyContaining<ProductValidator>();
        
        return service;
    }

    public static IServiceCollection AddMapper(this IServiceCollection service)
    {
        service.AddTransient<IMapper, Mapper>();
        TypeAdapterConfig.GlobalSettings.Scan(typeof(RecipeRegister).Assembly);
        return service;
    }
    
    public static IServiceCollection AddGrpcClients(this IServiceCollection service, ConfigurationManager configuration)
    {
        service.AddSingleton<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteProduct.ProductClient>(o =>
                o.Address = new Uri(configuration.GetServiceUri("product")!))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteCountry.CountryClient>(o =>
                o.Address = new Uri(configuration.GetServiceUri("country")!))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteSeller.SellerClient>(o =>
                o.Address = new Uri(configuration.GetServiceUri("seller")!))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteShopping.ShoppingClient>(o =>
                o.Address = new Uri(configuration.GetServiceUri("shopping")!))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteRecipe.RecipeClient>(o =>
                o.Address = new Uri(configuration.GetServiceUri("recipe")!))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        return service;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection service)
    {
        service.AddSingleton<ISellerService, SellerService>();
        service.AddSingleton<IProductService, ProductService>();
        service.AddSingleton<ICountryService, CountryService>();
        service.AddSingleton<IShoppingService, ShoppingService>();

        service.AddSingleton<IIdentityService, IdentityService>();

        return service;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection service)
    {
        service.AddTransient<AuthorizationTokenSwippingMiddleware>();

        service.AddTransient<UserAppendingMiddleware>();
        return service;
    }

    public static void AddSecurity(this IServiceCollection service, IConfiguration configuration)
    {
        var audience = configuration["Keycloak:resource"];
        service.AddTransient<IClaimsTransformation>(_ => new CustomKeycloakClaimsTransformation("role", audience));
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.Authority = configuration["Keycloak:auth-server-url"];
                o.Audience = audience;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience =
                        bool.TryParse(configuration["Keycloak:verify-token-audience"], out var shouldValidate) &&
                        shouldValidate,
                    ValidateIssuer = true,
                    NameClaimType = "preferred_username",
                    RoleClaimType = "role"
                };
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
            });

        service.AddAuthorization(o => o.AddPolicy("fed-regular", p =>
        {
            p.RequireAssertion(c => c.User.HasClaim(cl => cl.Value == "User"));
        }));
    }

    private static HttpClientHandler LoadDefaultClientHandler(ConfigurationManager configuration)
    {
        var handler = new HttpClientHandler();
        var cert = new X509Certificate2(File.ReadAllBytes(configuration.GetServicesCertPath()));
        handler.ClientCertificates.Add(cert);
        return handler;
    }
}