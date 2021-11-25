using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_ENCODING_TYPE
        {
        CAPICOM_ENCODE_BASE64,
        CAPICOM_ENCODE_BINARY,
        CAPICOM_ENCODE_ANY = -1
        }
    }
