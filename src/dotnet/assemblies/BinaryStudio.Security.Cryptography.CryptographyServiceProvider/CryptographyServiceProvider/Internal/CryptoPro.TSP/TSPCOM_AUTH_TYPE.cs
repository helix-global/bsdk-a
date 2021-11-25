using System;
using System.Diagnostics.CodeAnalysis;

namespace CryptoPro.TSP
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum TSPCOM_AUTH_TYPE
        {
        TSPCOM_AUTH_TYPE_ANONYMOUS,
        TSPCOM_AUTH_TYPE_BASIC,
        TSPCOM_AUTH_TYPE_DIGEST,
        TSPCOM_AUTH_TYPE_NTLM,
        TSPCOM_AUTH_TYPE_NEGOTIATE
        }
    }
