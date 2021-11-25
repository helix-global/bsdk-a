using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_CERTIFICATE_SAVE_AS_TYPE
        {
        CAPICOM_CERTIFICATE_SAVE_AS_PFX,
        CAPICOM_CERTIFICATE_SAVE_AS_CER
        }
    }
