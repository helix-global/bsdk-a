using System;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public interface ICmsIssuerAndSerialNumber
        {
        String CertificateSerialNumber { get; }
        IX509GeneralName CertificateIssuer { get; }
        }
    }