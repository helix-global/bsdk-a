using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateChain : IX509CertificateChainStatus,IList<IX509CertificateChainElement>
        {
        Int32 ChainIndex { get; }
        }
    }