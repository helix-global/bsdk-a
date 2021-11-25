using System;
using System.Diagnostics.CodeAnalysis;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CAPICOM_OID
        {
        CAPICOM_OID_OTHER,
        CAPICOM_OID_AUTHORITY_KEY_IDENTIFIER_EXTENSION,
        CAPICOM_OID_KEY_ATTRIBUTES_EXTENSION,
        CAPICOM_OID_CERT_POLICIES_95_EXTENSION,
        CAPICOM_OID_KEY_USAGE_RESTRICTION_EXTENSION,
        CAPICOM_OID_LEGACY_POLICY_MAPPINGS_EXTENSION,
        CAPICOM_OID_SUBJECT_ALT_NAME_EXTENSION,
        CAPICOM_OID_ISSUER_ALT_NAME_EXTENSION,
        CAPICOM_OID_BASIC_CONSTRAINTS_EXTENSION,
        CAPICOM_OID_SUBJECT_KEY_IDENTIFIER_EXTENSION,
        CAPICOM_OID_KEY_USAGE_EXTENSION,
        CAPICOM_OID_PRIVATEKEY_USAGE_PERIOD_EXTENSION,
        CAPICOM_OID_SUBJECT_ALT_NAME2_EXTENSION,
        CAPICOM_OID_ISSUER_ALT_NAME2_EXTENSION,
        CAPICOM_OID_BASIC_CONSTRAINTS2_EXTENSION,
        CAPICOM_OID_NAME_CONSTRAINTS_EXTENSION,
        CAPICOM_OID_CRL_DIST_POINTS_EXTENSION,
        CAPICOM_OID_CERT_POLICIES_EXTENSION,
        CAPICOM_OID_POLICY_MAPPINGS_EXTENSION,
        CAPICOM_OID_AUTHORITY_KEY_IDENTIFIER2_EXTENSION,
        CAPICOM_OID_POLICY_CONSTRAINTS_EXTENSION,
        CAPICOM_OID_ENHANCED_KEY_USAGE_EXTENSION,
        CAPICOM_OID_CERTIFICATE_TEMPLATE_EXTENSION,
        CAPICOM_OID_APPLICATION_CERT_POLICIES_EXTENSION,
        CAPICOM_OID_APPLICATION_POLICY_MAPPINGS_EXTENSION,
        CAPICOM_OID_APPLICATION_POLICY_CONSTRAINTS_EXTENSION,
        CAPICOM_OID_AUTHORITY_INFO_ACCESS_EXTENSION,
        CAPICOM_OID_SERVER_AUTH_EKU = 100,
        CAPICOM_OID_CLIENT_AUTH_EKU,
        CAPICOM_OID_CODE_SIGNING_EKU,
        CAPICOM_OID_EMAIL_PROTECTION_EKU,
        CAPICOM_OID_IPSEC_END_SYSTEM_EKU,
        CAPICOM_OID_IPSEC_TUNNEL_EKU,
        CAPICOM_OID_IPSEC_USER_EKU,
        CAPICOM_OID_TIME_STAMPING_EKU,
        CAPICOM_OID_CTL_USAGE_SIGNING_EKU,
        CAPICOM_OID_TIME_STAMP_SIGNING_EKU,
        CAPICOM_OID_SERVER_GATED_CRYPTO_EKU,
        CAPICOM_OID_ENCRYPTING_FILE_SYSTEM_EKU,
        CAPICOM_OID_EFS_RECOVERY_EKU,
        CAPICOM_OID_WHQL_CRYPTO_EKU,
        CAPICOM_OID_NT5_CRYPTO_EKU,
        CAPICOM_OID_OEM_WHQL_CRYPTO_EKU,
        CAPICOM_OID_EMBEDED_NT_CRYPTO_EKU,
        CAPICOM_OID_ROOT_LIST_SIGNER_EKU,
        CAPICOM_OID_QUALIFIED_SUBORDINATION_EKU,
        CAPICOM_OID_KEY_RECOVERY_EKU,
        CAPICOM_OID_DIGITAL_RIGHTS_EKU,
        CAPICOM_OID_LICENSES_EKU,
        CAPICOM_OID_LICENSE_SERVER_EKU,
        CAPICOM_OID_SMART_CARD_LOGON_EKU,
        CAPICOM_OID_PKIX_POLICY_QUALIFIER_CPS,
        CAPICOM_OID_PKIX_POLICY_QUALIFIER_USERNOTICE
        }
    }