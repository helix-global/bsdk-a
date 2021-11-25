using System.Numerics;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public interface ICmsIssuerAndSerialNumber
        {
        BigInteger CertificateSerialNumber { get; }
        IX509GeneralName CertificateIssuer { get; }
        }
    }