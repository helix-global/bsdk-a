using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CRYPT_KEY_SPEC
        {
        NONE        = 0,
        EXCHANGE    = 1,
        SIGNATURE   = 2
        }
    }
