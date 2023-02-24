using fed.cloud.common.Models;

namespace fedstocks.cloud.web.api.Extensions;

internal static class ConfigurationExtensions
{
    internal static string GetDefaultUrls(this ConfigurationManager configuration)
    {
        return configuration.GetSection("Urls").Value;
    }
    
    internal static string? GetServiceUri(this ConfigurationManager conf, string service)
    {
        try
        {
            return conf.GetSection("Microservices")?[service];
        }
        catch (Exception e)
        {
            return "https://localhost:9999";
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