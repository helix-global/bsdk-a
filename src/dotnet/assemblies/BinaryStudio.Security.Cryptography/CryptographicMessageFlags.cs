using System;

namespace BinaryStudio.Security.Cryptography
    {
    [Flags]
    public enum CryptographicMessageFlags
        {
        Attached = 1,
        Detached = 2,
        IncludeSigningCertificate = 4,
        IndefiniteLength = 8
        }
    }