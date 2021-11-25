using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_STORE_LOCATION
        {
        CAPICOM_MEMORY_STORE,
        CAPICOM_LOCAL_MACHINE_STORE,
        CAPICOM_CURRENT_USER_STORE,
        CAPICOM_ACTIVE_DIRECTORY_USER_STORE,
        CAPICOM_SMART_CARD_USER_STORE
        }
    }
