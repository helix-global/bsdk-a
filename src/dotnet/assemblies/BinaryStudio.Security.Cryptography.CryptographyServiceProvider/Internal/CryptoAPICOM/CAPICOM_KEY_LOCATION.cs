using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_KEY_LOCATION
        {
        CAPICOM_CURRENT_USER_KEY,
        CAPICOM_LOCAL_MACHINE_KEY
        }
    }
