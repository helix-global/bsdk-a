using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_STORE_OPEN_MODE
        {
        CAPICOM_STORE_OPEN_READ_ONLY,
        CAPICOM_STORE_OPEN_READ_WRITE,
        CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED,
        CAPICOM_STORE_OPEN_EXISTING_ONLY = 128,
        CAPICOM_STORE_OPEN_INCLUDE_ARCHIVED = 256
        }
    }
