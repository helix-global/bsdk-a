using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_CHECK_FLAG
        {
        CAPICOM_CHECK_NONE,
        CAPICOM_CHECK_TRUSTED_ROOT,
        CAPICOM_CHECK_TIME_VALIDITY,
        CAPICOM_CHECK_SIGNATURE_VALIDITY = 4,
        CAPICOM_CHECK_ONLINE_REVOCATION_STATUS = 8,
        CAPICOM_CHECK_OFFLINE_REVOCATION_STATUS = 16,
        CAPICOM_CHECK_COMPLETE_CHAIN = 32,
        CAPICOM_CHECK_NAME_CONSTRAINTS = 64,
        CAPICOM_CHECK_BASIC_CONSTRAINTS = 128,
        CAPICOM_CHECK_NESTED_VALIDITY_PERIOD = 256,
        CAPICOM_CHECK_ONLINE_ALL = 495,
        CAPICOM_CHECK_OFFLINE_ALL = 503
        }
    }
