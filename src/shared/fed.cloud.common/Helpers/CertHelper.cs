using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace fed.cloud.common.Helpers;

public static class CertHelper
{
    public static X509Certificate2 GetCertificate(string certPath, string keyPath)
    {
        var sslCert = CreateFromPublicPrivateKey(certPath, keyPath);
        
        return new X509Certificate2(sslCert.Export(X509ContentType.Pkcs12));
    }

    private static X509Certificate2 CreateFromPublicPrivateKey(string publicCert, string privateCert)
    {
        var publicPemBytes = File.ReadAllBytes(publicCert);
        using var publicX509 = new X509Certificate2(publicPemBytes);
        var privateKeyText = File.ReadAllText(privateCert);
        var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);

        using var rsa = RSA.Create();
        switch (privateKeyBlocks[0])
        {
            case "BEGIN PRIVATE KEY":
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                break;
            case "BEGIN RSA PRIVATE KEY":
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                break;
        }
        var keyPair = publicX509.CopyWithPrivateKey(rsa);
        return keyPair;
    }
}