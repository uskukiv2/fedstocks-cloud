using fedstocks.cloud.web.api.Extensions;
using fedstocks.cloud.web.api.Models.Configurations;
using fedstocks.cloud.web.api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using System.Net;
using fedstocks.cloud.web.api.Infrastructure.Middlewares;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(builder.Configuration.GetDefaultUrls());

var identityConfiguration = new IdentityConfiguration();
builder.Configuration.Bind(nameof(IdentityConfiguration), identityConfiguration);

// Add services to the container.

builder.Services.AddLogging(x =>
{
    x.SetMinimumLevel(builder.Configuration.GetDefaultLogLevel());
    if (!builder.Environment.IsDevelopment())
    {
        //TODO: FED-129 Implement remote logging service
    }
});

//configurion registration
builder.Services.AddSingleton<IdentityConfiguration>(sp => identityConfiguration);

builder.Services.AddCustomServices();
builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddHttpLogging(x =>
{
    x.LoggingFields = builder.Environment.IsDevelopment() 
        ? HttpLoggingFields.All : HttpLoggingFields.RequestMethod;
});
builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddMiddlewares();
builder.Services.AddValidation();
builder.Services.AddMapper();

builder.Services.AddSwaggerDocument();

var identityUrl = identityConfiguration.IdentityUrl;
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/gen_app"))
    .SetApplicationName("gen_apps");
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = $".gen_apps";
    options.Cookie.Domain = "localhost";
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
        .AddCookie(setup =>
        {
            setup.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var schemeHost = identityUrl.Split("://");
            var scheme = schemeHost[0];
            var hostPort = schemeHost[1].Split(":");
            var host = hostPort[0];
            var port = hostPort[1].Replace("/", "");
            //options.Authority = GetUrl(scheme, host, int.Parse(port));
            options.Audience = "gen_apps_shoppera";
            options.ForwardSignIn = GetUrl(scheme, host, int.Parse(port), "Login");
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidAudiences = new[] { "gen_apps_shoppera" },
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuers = new[] { identityUrl },
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
            };
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("jwt", o =>
    {
        o.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        o.Requirements.Add(new DenyAnonymousAuthorizationRequirement());
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapControllers();
app.UseHttpLogging();

app.UseOpenApi();
app.UseSwaggerUi3();

var routeBuilder = new RouteBuilder(app);
routeBuilder.MapMiddlewareRoute("api/{controller}/{action}", appBuilder =>
{
    appBuilder.UseMiddleware<AuthorizationTokenSwippingMiddleware>();
    appBuilder.UseMiddleware<UserAppendingMiddleware>();
    appBuilder.UseAuthentication();
    appBuilder.UseStatusCodePages(context =>
    {
        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;
        if (response.StatusCode != (int)HttpStatusCode.Unauthorized)
        {
            return Task.CompletedTask;
        }

        var schemeHost = identityUrl.Split("://");
        var scheme = schemeHost[0];
        var hostPort = schemeHost[1].Split(":");
        var host = hostPort[0];
        var port = hostPort[1].Replace("/", "");
        response.Redirect(GetUrl(scheme, host, int.Parse(port), $"Login?ReturnUrl={app.Configuration.GetValue<string>("Urls")}"));

        return Task.CompletedTask;
    });
    appBuilder.UseRouting();
    appBuilder.UseAuthorization();
    appBuilder.UseEndpoints(endpoint =>
    {
        endpoint.MapControllers();
    });
});

app.UseExceptionHandler(exApp =>
{
    exApp.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandler?.Error.InnerException?.InnerException is RpcException rex)
        {
            switch (rex.StatusCode)
            {
                case StatusCode.Unavailable:
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    break;
                case StatusCode.Unauthenticated:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
            }

            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    });
});

app.UseRouter(routeBuilder.Build());

app.Run();


string GetUrl(string scheme, string host, int port, string path = null)
{
    var uri = new UriBuilder(scheme, host)
    {
        Port = port
    };
    if (!string.IsNullOrEmpty(path))
    {
        uri.Path = path;
    }

    return uri.ToString();
}
