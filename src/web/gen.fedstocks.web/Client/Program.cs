using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using gen.fedstocks.web.Client;
using gen.fedstocks.web.Client.Application.Extensions;
using gen.fedstocks.web.Client.Application.Models;
using gen.fedstocks.web.Client.Extensions;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var types = GetAssemblyTypes();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient(AppConstValues.ServerAPI,
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddMudServices();
builder.Services.AddApplication(types);
builder.Services.AddClient();

await builder.Build().RunAsync();

IReadOnlyCollection<Type> GetAssemblyTypes()
{
    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

    return new List<Type>(types).AsReadOnly();
}