using fedstocks.cloud.web.api.Extensions;
using fedstocks.cloud.web.api.Middleware;
using fedstocks.cloud.web.api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(builder.Configuration.GetDefaultUrls());

// Add services to the container.

builder.Services.AddLogging(x =>
{
    x.SetMinimumLevel(builder.Configuration.GetDefaultLogLevel());
    if (!builder.Environment.IsDevelopment())
    {
        //TODO: FED-129 Implement remote logging service
    }
});
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

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<DevelopmentClientAuthenticationMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHttpLogging();

app.UseOpenApi();
app.UseSwaggerUi3();

app.Run();
