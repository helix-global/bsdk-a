using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateRevocationList {
        IntPtr Handle { get; }
        String Thumbprint { get; }
        String Country { get; }
        DateTime  EffectiveDate { get; }
        DateTime? NextUpdate { get; }
        IX509RelativeDistinguishedNameSequence Issuer { get; }
        IList<IX509CertificateExtension> Extensions { get; }
        Byte[] SignatureValue { get; }
        Stream GetSigningStream();
        }
    }