using System.Reflection;
using fed.cloud.common.Models;
using fed.cloud.shopping.api.Models.Configuration;
using fedstocks.cloud.web.api.Models.Sections;

namespace fedstocks.cloud.web.api.Extensions;

internal static class ConfigurationExtensions
{
    internal static string GetDefaultUrls(this ConfigurationManager configuration)
    {
        return configuration.GetSection("Urls").Value;
    }

    internal static string GetProductServiceUriString(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("RemoteServices").Get<RemoteServices>().ProductServiceUri;
        }
        catch
        {
            return "https://localhost:9085/";
        }
    }

    internal static string GetCountryServiceUriString(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("RemoteServices").Get<RemoteServices>().CountryServiceUri;
        }
        catch
        {
            return "https://localhost:9085/";
        }
    }

    internal static string GetSellerServiceUriString(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("RemoteServices").Get<RemoteServices>().SellerServiceUri;
        }
        catch
        {
            return "https://localhost:9085/";
        }
    }

    internal static string GetShoppingServiceUriString(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("RemoteServices").Get<RemoteServices>().ShoppingServiceUri;
        }
        catch
        {
            return "https://localhost:9086/";
        }
    }

    internal static string GetServicesCertPath(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("Certification").Get<Certification>().CertificatePath;
        }
        catch
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}\\certificate.pem";
        }
    }

    internal static string GetServiceKeyCertPath(this ConfigurationManager configuration)
    {
        try
        {
            return configuration.GetSection("Certification").Get<Certification>().KeyCertificatePath;
        }
        catch
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}\\key.pem";
        }
    }

    internal static LogLevel GetDefaultLogLevel(this ConfigurationManager configuration)
    {
        return configuration.GetSection("Logging").GetSection("LogLevel").GetValue<LogLevel>("Default");
    }
}