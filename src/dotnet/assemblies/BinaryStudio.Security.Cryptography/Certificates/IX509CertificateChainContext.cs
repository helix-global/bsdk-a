using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateChainContext :
        IDisposable,
        IX509CertificateChainStatus,
        IList<IX509CertificateChain>
        {
        }
    }