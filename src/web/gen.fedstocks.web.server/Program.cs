using gen.fed.application.Extensions;
using gen.fedstocks.web.server.Extensions;

var builder = WebApplication.CreateBuilder(args);
var types = GetAssemblyTypes();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddApplication(types);

builder.Services.AddFed();
builder.Services.AddDatabase(builder.Configuration);

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
