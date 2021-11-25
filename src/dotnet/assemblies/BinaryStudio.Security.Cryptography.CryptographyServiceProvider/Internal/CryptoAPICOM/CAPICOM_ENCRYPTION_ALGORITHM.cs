using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_ENCRYPTION_ALGORITHM
        {
        CAPICOM_ENCRYPTION_ALGORITHM_RC2,
        CAPICOM_ENCRYPTION_ALGORITHM_RC4,
        CAPICOM_ENCRYPTION_ALGORITHM_DES,
        CAPICOM_ENCRYPTION_ALGORITHM_3DES,
        CAPICOM_ENCRYPTION_ALGORITHM_AES
        }
    }
