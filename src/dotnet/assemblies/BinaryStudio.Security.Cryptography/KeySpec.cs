using System;
using System.Diagnostics.CodeAnalysis;

namespace BinaryStudio.Security.Cryptography
    {
    [Flags]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1714:Flags enums should have plural names", Justification = "<Pending>")]
    #endif
    public enum KeySpec
        {
        None        = 0,
        Exchange    = 1,
        Signature   = 2
        }
    }