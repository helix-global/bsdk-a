using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum KP_PERMISSIONS : uint
        {
        CRYPT_ENCRYPT           = 0x0001,  // Allow encryption
        CRYPT_DECRYPT           = 0x0002,  // Allow decryption
        CRYPT_EXPORT            = 0x0004,  // Allow key to be exported
        CRYPT_READ              = 0x0008,  // Allow parameters to be read
        CRYPT_WRITE             = 0x0010,  // Allow parameters to be set
        CRYPT_MAC               = 0x0020,  // Allow MACs to be used with key
        CRYPT_EXPORT_KEY        = 0x0040,  // Allow key to be used for exporting keys
        CRYPT_IMPORT_KEY        = 0x0080,  // Allow key to be used for importing keys
        CRYPT_ARCHIVE           = 0x0100   // Allow key to be exported at creation only
        }
    }