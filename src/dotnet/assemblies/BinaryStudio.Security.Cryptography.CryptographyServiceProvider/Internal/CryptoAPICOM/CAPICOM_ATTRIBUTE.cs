using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_ATTRIBUTE
        {
        CAPICOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME,
        CAPICOM_AUTHENTICATED_ATTRIBUTE_DOCUMENT_NAME,
        CAPICOM_AUTHENTICATED_ATTRIBUTE_DOCUMENT_DESCRIPTION
        }
    }
