using System;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents.Win32;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateChainPolicy
        {
        CertificateChainPolicy Policy { get; }
        void Verify(ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext);
        }
    }