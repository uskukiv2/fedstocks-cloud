using gen.fed.ui.Extensions;
using gen.fedstocks.web.server.Data;
using gen.fedstocks.web.server.Extensions;
using gen.fedstocks.web.server.Services;

var builder = WebApplication.CreateBuilder(args);
var types = GetAssemblyTypes();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddUI(types);

builder.Services.AddFed();

builder.Services.AddSingleton<IPageManager, PageManager>();
builder.Services.AddSingleton<ITitleService, TitleService>();

builder.Services.AddScoped<ITopbarItemsService, TopbarItemsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

IReadOnlyCollection<Type> GetAssemblyTypes()
{
    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

    return new List<Type>(types).AsReadOnly();
}
