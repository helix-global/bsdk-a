using System;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [Flags]
    public enum NetscapeCertificateType
        {
        SslClient        = (1 << 7),
        SslServer        = (1 << 6),
        SMime            = (1 << 5),
        ObjectSigning    = (1 << 4),
        Reserved         = (1 << 3),
        SslCA            = (1 << 2),
        SMimeCA          = (1 << 1),
        ObjectSigningCA  = (1 << 0)
        }
    }