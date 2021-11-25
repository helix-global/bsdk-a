using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_CERT_INFO_TYPE
        {
        CAPICOM_CERT_INFO_SUBJECT_SIMPLE_NAME,
        CAPICOM_CERT_INFO_ISSUER_SIMPLE_NAME,
        CAPICOM_CERT_INFO_SUBJECT_EMAIL_NAME,
        CAPICOM_CERT_INFO_ISSUER_EMAIL_NAME,
        CAPICOM_CERT_INFO_SUBJECT_UPN,
        CAPICOM_CERT_INFO_ISSUER_UPN,
        CAPICOM_CERT_INFO_SUBJECT_DNS_NAME,
        CAPICOM_CERT_INFO_ISSUER_DNS_NAME
        }
    }
