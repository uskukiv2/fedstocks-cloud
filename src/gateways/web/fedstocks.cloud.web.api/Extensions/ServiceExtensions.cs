using fedstocks.cloud.web.api.Grpc;
using fedstocks.cloud.web.api.Services;
using fedstocks.cloud.web.api.Services.Implementation;
using System.Security.Cryptography.X509Certificates;
using RemoteCountry = fed.cloud.product.host.Protos.Country;
using RemoteProduct = fed.cloud.product.host.Protos.Product;
using RemoteSeller = fed.cloud.product.host.Protos.Seller;
using RemoteShopping = fed.cloud.shopping.api.Protos.Shopping;

namespace fedstocks.cloud.web.api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection service, ConfigurationManager configuration)
    {
        service.AddSingleton<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteProduct.ProductClient>("Product",
                options => options.Address = new Uri(configuration.GetProductServiceUriString()))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteCountry.CountryClient>(o => o.Address = new Uri(configuration.GetCountryServiceUriString()))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteSeller.SellerClient>(o => o.Address = new Uri(configuration.GetSellerServiceUriString()))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        service.AddGrpcClient<RemoteShopping.ShoppingClient>(o => o.Address = new Uri(configuration.GetShoppingServiceUriString()))
            .ConfigurePrimaryHttpMessageHandler(() => LoadDefaultClientHandler(configuration))
            .AddInterceptor<GlobalLoggingInterceptor>();

        return service;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection service)
    {
        service.AddSingleton<ISellerService, SellerService>();
        service.AddSingleton<IProductService, ProductService>();
        service.AddSingleton<ICountryService, CountryService>();
        service.AddSingleton<IShoppingService, ShoppingService>();

        service.AddSingleton<IIdentityService, IdentityService>();

        return service;
    }

    private static HttpClientHandler LoadDefaultClientHandler(ConfigurationManager configuration)
    {
        var handler = new HttpClientHandler();
        var cert = new X509Certificate2(File.ReadAllBytes(configuration.GetServicesCertPath()));
        handler.ClientCertificates.Add(cert);
        return handler;
    }
}