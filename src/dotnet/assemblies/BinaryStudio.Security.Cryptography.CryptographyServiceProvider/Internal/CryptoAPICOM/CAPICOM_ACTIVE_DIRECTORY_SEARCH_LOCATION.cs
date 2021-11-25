using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_ACTIVE_DIRECTORY_SEARCH_LOCATION
        {
        CAPICOM_SEARCH_ANY,
        CAPICOM_SEARCH_GLOBAL_CATALOG,
        CAPICOM_SEARCH_DEFAULT_DOMAIN
        }
    }
