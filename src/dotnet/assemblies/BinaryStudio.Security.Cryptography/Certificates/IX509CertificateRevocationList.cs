using System;
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
        Byte[] SignatureValue { get; }
        Stream GetSigningStream();
        }
    }