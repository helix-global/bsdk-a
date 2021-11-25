using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Flags]
    public enum X509OpenFlags
        {
        ReadOnly            = 0,
        ReadWrite           = 1,
        MaxAllowed          = 2,
        OpenExistingOnly    = 4,
        IncludeArchived     = 8
        }
    }