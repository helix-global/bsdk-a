using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_KEY_STORAGE_FLAG
        {
        CAPICOM_KEY_STORAGE_DEFAULT,
        CAPICOM_KEY_STORAGE_EXPORTABLE,
        CAPICOM_KEY_STORAGE_USER_PROTECTED
        }
    }
