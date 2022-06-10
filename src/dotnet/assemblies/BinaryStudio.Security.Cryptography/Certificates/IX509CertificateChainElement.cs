using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateChainElement:
        IX509CertificateChainStatus
        {
        IX509Certificate Certificate { get; }
        IX509CertificateRevocationList CertificateRevocationList { get; }
        Int32 ElementIndex { get; }
        }
    }