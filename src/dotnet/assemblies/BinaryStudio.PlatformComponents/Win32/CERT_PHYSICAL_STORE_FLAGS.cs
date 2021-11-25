using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CERT_PHYSICAL_STORE_FLAGS
        {
        CERT_PHYSICAL_STORE_ADD_ENABLE_FLAG                     = 0x1,
        CERT_PHYSICAL_STORE_OPEN_DISABLE_FLAG                   = 0x2,
        CERT_PHYSICAL_STORE_REMOTE_OPEN_DISABLE_FLAG            = 0x4,
        CERT_PHYSICAL_STORE_INSERT_COMPUTER_NAME_ENABLE_FLAG    = 0x8
        }
    }