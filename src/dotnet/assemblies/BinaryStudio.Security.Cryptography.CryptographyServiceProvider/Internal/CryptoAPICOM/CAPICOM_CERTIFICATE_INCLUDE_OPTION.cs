using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_CERTIFICATE_INCLUDE_OPTION
        {
        CAPICOM_CERTIFICATE_INCLUDE_CHAIN_EXCEPT_ROOT,
        CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN,
        CAPICOM_CERTIFICATE_INCLUDE_END_ENTITY_ONLY
        }
    }
