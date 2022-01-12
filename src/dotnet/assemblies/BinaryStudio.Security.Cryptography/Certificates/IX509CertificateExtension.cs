using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateExtension
        {
        Boolean IsCritical { get; }
        IObjectIdentifier Identifier { get; }
        }
    }