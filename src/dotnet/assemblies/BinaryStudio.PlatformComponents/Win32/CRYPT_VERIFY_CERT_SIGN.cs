using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Win32
    {
    [Flags]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CRYPT_VERIFY_CERT_SIGN : uint
        {
        NONE = 0,
        CRYPT_VERIFY_CERT_SIGN_DISABLE_MD2_MD4_FLAG          = 0x00000001,
        CRYPT_VERIFY_CERT_SIGN_SET_STRONG_PROPERTIES_FLAG    = 0x00000002,
        CRYPT_VERIFY_CERT_SIGN_RETURN_STRONG_PROPERTIES_FLAG = 0x00000004,
        CRYPT_VERIFY_CERT_SIGN_CHECK_WEAK_HASH_FLAG          = 0x00000008
        }
    }