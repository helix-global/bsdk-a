using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateChainPolicy
        {
        Boolean Verify(ISet<Exception> target, ICryptographicContext context,
            OidCollection applicationpolicy, OidCollection certificatepolicy,
            TimeSpan timeout, DateTime datetime, IX509CertificateStorage store,
            CERT_CHAIN_FLAGS flags, ref CERT_CHAIN_CONTEXT chaincontext);
        }
    }