using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_EKU
        {
        OTHER,
        SERVER_AUTH,
        CLIENT_AUTH,
        CODE_SIGNING,
        EMAIL_PROTECTION,
        SMARTCARD_LOGON,
        ENCRYPTING_FILE_SYSTEM
        }
    }
