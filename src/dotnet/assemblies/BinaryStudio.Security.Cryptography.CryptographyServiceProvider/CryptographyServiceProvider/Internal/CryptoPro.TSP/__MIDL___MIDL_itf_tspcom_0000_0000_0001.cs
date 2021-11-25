using System;
using System.Diagnostics.CodeAnalysis;

namespace CryptoPro.TSP
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum __MIDL___MIDL_itf_tspcom_0000_0000_0001
        {
        TSPCOM_AUTH_TYPE_ANONYMOUS,
        TSPCOM_AUTH_TYPE_BASIC,
        TSPCOM_AUTH_TYPE_DIGEST,
        TSPCOM_AUTH_TYPE_NTLM,
        TSPCOM_AUTH_TYPE_NEGOTIATE
        }
    }
