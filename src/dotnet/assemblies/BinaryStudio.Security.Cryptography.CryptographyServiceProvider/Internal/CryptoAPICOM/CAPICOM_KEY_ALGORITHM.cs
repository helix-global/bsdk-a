using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_KEY_ALGORITHM
        {
        CAPICOM_KEY_ALGORITHM_OTHER,
        CAPICOM_KEY_ALGORITHM_RSA,
        CAPICOM_KEY_ALGORITHM_DSS
        }
    }
