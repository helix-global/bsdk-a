using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_KEY_USAGE
        {
        CAPICOM_DIGITAL_SIGNATURE_KEY_USAGE = 128,
        CAPICOM_NON_REPUDIATION_KEY_USAGE = 64,
        CAPICOM_KEY_ENCIPHERMENT_KEY_USAGE = 32,
        CAPICOM_DATA_ENCIPHERMENT_KEY_USAGE = 16,
        CAPICOM_KEY_AGREEMENT_KEY_USAGE = 8,
        CAPICOM_KEY_CERT_SIGN_KEY_USAGE = 4,
        CAPICOM_OFFLINE_CRL_SIGN_KEY_USAGE = 2,
        CAPICOM_CRL_SIGN_KEY_USAGE = 2,
        CAPICOM_ENCIPHER_ONLY_KEY_USAGE = 1,
        CAPICOM_DECIPHER_ONLY_KEY_USAGE = 32768
        }
    }
