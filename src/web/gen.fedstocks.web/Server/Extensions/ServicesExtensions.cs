using gen.fed.web.domain.Abstract;
using gen.fed.web.domain.Factories;
using gen.fed.web.domain.Repositories;
using gen.fed.web.infrastructure;
using gen.fed.web.infrastructure.Factories;
using gen.fed.web.infrastructure.Repositories;
using gen.fedstocks.web.Server.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace gen.fedstocks.web.Server.Extensions;

public static class ServiceExtensions
{
    public static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var audience = configuration["Keycloak:resource"];
        services.AddTransient<IClaimsTransformation>(_ => new CustomKeycloakClaimsTransformation("role", audience));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        services.AddAuthorization(o => o.AddPolicy("fed-regular", p =>
        {
            p.RequireAssertion(c => c.User.HasClaim(cl => cl.Value == "User"));
        }));
    }

    public static void AddExplorer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        var oidcConnectUrl = $"{configuration["Keycloak:auth-server-url"]}" +
                             $"realms/{configuration["Keycloak:realm"]}/" +
                             ".well-known/openid-configuration";

        services.AddSwaggerGen(c =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Auth",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri(oidcConnectUrl),
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
        });
    }
    
    public static IServiceCollection AddFed(this IServiceCollection services)
    {
        RegisterRepositories(services);

        RegisterFactories(services);

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service, ConfigurationManager config)
    {
        service.AddEntityFrameworkNpgsql()
            .AddDbContext<ServiceContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("default"));
            });
        service.AddScoped<IUnitOfWork<NpgsqlConnection>>(sp => sp.GetRequiredService<ServiceContext>());
        return service;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void RegisterFactories(IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
    }
}
