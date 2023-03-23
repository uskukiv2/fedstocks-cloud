using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using gen.fedstocks.web.Client;
using gen.fedstocks.web.Client.Application.Extensions;
using gen.fedstocks.web.Client.Application.Models;
using gen.fedstocks.web.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var types = GetAssemblyTypes();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddHttpClient(AppConstValues.ServerAPI,
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(AppConstValues.ServerAPI));

builder.Services.AddOidcAuthentication(o =>
{
    o.ProviderOptions.MetadataUrl = "http://localhost:8080/realms/fed/.well-known/openid-configuration";
    o.ProviderOptions.Authority = "http://localhost:8080/realms/fed";
    o.ProviderOptions.ClientId = "fed-client";
    o.ProviderOptions.ResponseType = "id_token token";

    o.UserOptions.NameClaim = "preferred_username";
    o.UserOptions.RoleClaim = "roles";
    o.UserOptions.ScopeClaim = "scope";
});

builder.Services.AddApplication(types);
builder.Services.AddClient();

await builder.Build().RunAsync();

IReadOnlyCollection<Type> GetAssemblyTypes()
{
    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

    return new List<Type>(types).AsReadOnly();
}