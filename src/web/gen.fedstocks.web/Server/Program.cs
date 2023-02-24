using gen.fedstocks.web.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging(cfg =>
{
    cfg.SetMinimumLevel(LogLevel.Trace);
    cfg.AddDebug();
    cfg.AddConsole();
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddFed();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseEndpoints(end =>
{
    end.MapReverseProxy();
});

app.Run();
