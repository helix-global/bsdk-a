using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Flags]
    public enum X509KeySpec
        {
        None        = 0,
        Exchange    = 1,
        Signature   = 2
        }
    }