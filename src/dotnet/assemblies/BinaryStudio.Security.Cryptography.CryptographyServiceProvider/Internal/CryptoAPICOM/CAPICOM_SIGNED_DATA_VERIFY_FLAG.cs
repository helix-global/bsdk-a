using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_SIGNED_DATA_VERIFY_FLAG
        {
        CAPICOM_VERIFY_SIGNATURE_ONLY,
        CAPICOM_VERIFY_SIGNATURE_AND_CERTIFICATE
        }
    }
