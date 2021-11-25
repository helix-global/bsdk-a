using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_STORE_SAVE_AS_TYPE
        {
        CAPICOM_STORE_SAVE_AS_SERIALIZED,
        CAPICOM_STORE_SAVE_AS_PKCS7
        }
    }
