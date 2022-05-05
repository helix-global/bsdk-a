using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateExtension : IDisposable
        {
        Boolean IsCritical { get; }
        IObjectIdentifier Identifier { get; }
        }
    }