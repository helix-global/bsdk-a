using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_KEY_SPEC
        {
        CAPICOM_KEY_SPEC_KEYEXCHANGE = 1,
        CAPICOM_KEY_SPEC_SIGNATURE
        }
    }
