using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CRYPT_FLAGS : uint
        {
        CRYPT_NONE         = 0x00000000,
        CRYPT_NOHASHOID    = 0x00000001,
        CRYPT_TYPE2_FORMAT = 0x00000002,
        CRYPT_X931_FORMAT  = 0x00000004
        }
    }