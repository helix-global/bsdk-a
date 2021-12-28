using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509Certificate
        {
        Int32 Version { get; }
        String SerialNumber { get; }
        String Container { get; }
        String Issuer { get; }
        String Subject { get; }
        String Thumbprint { get; }
        String FriendlyName { get; }
        Oid SignatureAlgorithm { get; }
        Oid HashAlgorithm { get; }
        DateTime NotAfter { get; }
        DateTime NotBefore { get; }
        IntPtr Handle { get; }
        X509KeySpec KeySpec { get; }
        Byte[] Bytes { get; }
        Byte[] SignatureValue { get; }
        String Country { get; }
        PublicKey PublicKey { get; }
        void Verify(ICryptographicContext context, IX509CertificateChainPolicy policy);
        void VerifyPrivateKeyUsagePeriod();
        Stream GetSigningStream();
        }
    }