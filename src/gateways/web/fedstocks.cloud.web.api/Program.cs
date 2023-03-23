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

builder.Services.AddSecurity(builder.Configuration);
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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapControllers();
app.UseHttpLogging();

app.UseOpenApi();
app.UseSwaggerUi3();

var routeBuilder = new RouteBuilder(app);
routeBuilder.MapMiddlewareRoute("cloud/api/{controller}/{action}", appBuilder =>
{
    appBuilder.UseMiddleware<AuthorizationTokenSwippingMiddleware>();
    appBuilder.UseMiddleware<UserAppendingMiddleware>();

    appBuilder.UseRouting();

    appBuilder.UseAuthentication();
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
