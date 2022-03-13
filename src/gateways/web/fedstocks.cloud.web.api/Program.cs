using fedstocks.cloud.web.api.Extensions;
using fedstocks.cloud.web.api.Middleware;
using fedstocks.cloud.web.api.Models.Configurations;
using fedstocks.cloud.web.api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


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
builder.Services.AddControllers();
builder.Services.AddHttpLogging(x =>
{
    x.LoggingFields = HttpLoggingFields.All;
});
builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddFluentValidation();
builder.Services.AddValidatorsFromAssembly(typeof(NewShoppingListValidator).Assembly, includeInternalTypes: true);
builder.Services.AddSwaggerDocument();

builder.Services.AddTransient<AuthorizationTokenSwippingMiddleware>();

builder.Services.AddTransient<UserAppendingMiddleware>();

var identityUrl = builder.Configuration.GetValue<string>("IdentityUrl");
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
            options.Authority = identityUrl;
            options.ForwardSignIn = $"{identityUrl}Login";
            options.RequireHttpsMetadata = false;
        });

var app = builder.Build();

// Configure the HTTP request pipeline.

//if (app.Environment.IsDevelopment())
//{
//    app.UseMiddleware<DevelopmentClientAuthenticationMiddleware>();
//}

app.UseHttpsRedirection();

app.UseMiddleware<AuthorizationTokenSwippingMiddleware>();

app.UseMiddleware<UserAppendingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHttpLogging();

app.UseOpenApi();
app.UseSwaggerUi3();

app.Run();
