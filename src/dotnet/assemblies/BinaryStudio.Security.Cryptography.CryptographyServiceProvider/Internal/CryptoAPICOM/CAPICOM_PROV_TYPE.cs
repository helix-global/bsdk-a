using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_PROV_TYPE
        {
        CAPICOM_PROV_RSA_FULL = 1,
        CAPICOM_PROV_RSA_SIG,
        CAPICOM_PROV_DSS,
        CAPICOM_PROV_FORTEZZA,
        CAPICOM_PROV_MS_EXCHANGE,
        CAPICOM_PROV_SSL,
        CAPICOM_PROV_RSA_SCHANNEL = 12,
        CAPICOM_PROV_DSS_DH,
        CAPICOM_PROV_EC_ECDSA_SIG,
        CAPICOM_PROV_EC_ECNRA_SIG,
        CAPICOM_PROV_EC_ECDSA_FULL,
        CAPICOM_PROV_EC_ECNRA_FULL,
        CAPICOM_PROV_DH_SCHANNEL,
        CAPICOM_PROV_SPYRUS_LYNKS = 20,
        CAPICOM_PROV_RNG,
        CAPICOM_PROV_INTEL_SEC,
        CAPICOM_PROV_REPLACE_OWF,
        CAPICOM_PROV_RSA_AES
        }
    }
