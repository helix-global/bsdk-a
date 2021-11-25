using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_HASH_ALGORITHM
        {
        CAPICOM_HASH_ALGORITHM_SHA1,
        CAPICOM_HASH_ALGORITHM_MD2,
        CAPICOM_HASH_ALGORITHM_MD4,
        CAPICOM_HASH_ALGORITHM_MD5,
        CAPICOM_HASH_ALGORITHM_SHA_256,
        CAPICOM_HASH_ALGORITHM_SHA_384,
        CAPICOM_HASH_ALGORITHM_SHA_512
        }
    }
