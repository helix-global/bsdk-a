using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_ENCRYPTION_KEY_LENGTH
        {
        CAPICOM_ENCRYPTION_KEY_LENGTH_MAXIMUM,
        CAPICOM_ENCRYPTION_KEY_LENGTH_40_BITS,
        CAPICOM_ENCRYPTION_KEY_LENGTH_56_BITS,
        CAPICOM_ENCRYPTION_KEY_LENGTH_128_BITS,
        CAPICOM_ENCRYPTION_KEY_LENGTH_192_BITS,
        CAPICOM_ENCRYPTION_KEY_LENGTH_256_BITS
        }
    }
