using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    #endif
    public abstract class Constants
        {
        public const Int32 CAPICOM_MAJOR_VERSION = 2;
        public const Int32 CAPICOM_MINOR_VERSION = 1;
        public const Int32 CAPICOM_RELEASE_NUMBER = 0;
        public const Int32 CAPICOM_BUILD_NUMBER = 2;

        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_VERSION_INFO = "Internal.CryptoAPICOM v2.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_COPY_RIGHT = "Copyright (c) Microsoft Corporation 1999-2006. All rights reserved.";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_MY_STORE = "My";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CA_STORE = "Ca";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ROOT_STORE = "Root";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OTHER_STORE = "AddressBook";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_SERVER_AUTH = "1.3.6.1.5.5.7.3.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_CODE_SIGNING = "1.3.6.1.5.5.7.3.3";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_EMAIL_PROTECTION = "1.3.6.1.5.5.7.3.4";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_IPSEC_END_SYSTEM = "1.3.6.1.5.5.7.3.5";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_IPSEC_TUNNEL = "1.3.6.1.5.5.7.3.6";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_IPSEC_USER = "1.3.6.1.5.5.7.3.7";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_TIME_STAMPING = "1.3.6.1.5.5.7.3.8";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_CTL_USAGE_SIGNING = "1.3.6.1.4.1.311.10.3.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_TIME_STAMP_SIGNING = "1.3.6.1.4.1.311.10.3.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_SERVER_GATED_CRYPTO = "1.3.6.1.4.1.311.10.3.3";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_ENCRYPTING_FILE_SYSTEM = "1.3.6.1.4.1.311.10.3.4";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_EFS_RECOVERY = "1.3.6.1.4.1.311.10.3.4.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_WHQL_CRYPTO = "1.3.6.1.4.1.311.10.3.5";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_NT5_CRYPTO = "1.3.6.1.4.1.311.10.3.6";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_OEM_WHQL_CRYPTO = "1.3.6.1.4.1.311.10.3.7";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_EMBEDED_NT_CRYPTO = "1.3.6.1.4.1.311.10.3.8";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_ROOT_LIST_SIGNER = "1.3.6.1.4.1.311.10.3.9";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_QUALIFIED_SUBORDINATION = "1.3.6.1.4.1.311.10.3.10";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_KEY_RECOVERY = "1.3.6.1.4.1.311.10.3.11";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_DIGITAL_RIGHTS = "1.3.6.1.4.1.311.10.5.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_LICENSES = "1.3.6.1.4.1.311.10.6.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_LICENSE_SERVER = "1.3.6.1.4.1.311.10.6.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OID_SMART_CARD_LOGON = "1.3.6.1.4.1.311.20.2.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_SERVER_AUTH_OID = "1.3.6.1.5.5.7.3.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CLIENT_AUTH_OID = "1.3.6.1.5.5.7.3.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CODE_SIGNING_OID = "1.3.6.1.5.5.7.3.3";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_EMAIL_PROTECTION_OID = "1.3.6.1.5.5.7.3.4";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_IPSEC_END_SYSTEM_OID = "1.3.6.1.5.5.7.3.5";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_IPSEC_TUNNEL_OID = "1.3.6.1.5.5.7.3.6";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_IPSEC_USER_OID = "1.3.6.1.5.5.7.3.7";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_TIME_STAMPING_OID = "1.3.6.1.5.5.7.3.8";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CTL_USAGE_SIGNING_OID = "1.3.6.1.4.1.311.10.3.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_TIME_STAMP_SIGNING_OID = "1.3.6.1.4.1.311.10.3.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_SERVER_GATED_CRYPTO_OID = "1.3.6.1.4.1.311.10.3.3";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ENCRYPTING_FILE_SYSTEM_OID = "1.3.6.1.4.1.311.10.3.4";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_EFS_RECOVERY_OID = "1.3.6.1.4.1.311.10.3.4.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_WHQL_CRYPTO_OID = "1.3.6.1.4.1.311.10.3.5";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_NT5_CRYPTO_OID = "1.3.6.1.4.1.311.10.3.6";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_OEM_WHQL_CRYPTO_OID = "1.3.6.1.4.1.311.10.3.7";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_EMBEDED_NT_CRYPTO_OID = "1.3.6.1.4.1.311.10.3.8";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ROOT_LIST_SIGNER_OID = "1.3.6.1.4.1.311.10.3.9";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_QUALIFIED_SUBORDINATION_OID = "1.3.6.1.4.1.311.10.3.10";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_KEY_RECOVERY_OID = "1.3.6.1.4.1.311.10.3.11";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_DIGITAL_RIGHTS_OID = "1.3.6.1.4.1.311.10.5.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_LICENSES_OID = "1.3.6.1.4.1.311.10.6.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_LICENSE_SERVER_OID = "1.3.6.1.4.1.311.10.6.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_SMART_CARD_LOGON_OID = "1.3.6.1.4.1.311.20.2.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ANY_APPLICATION_POLICY_OID = "1.3.6.1.4.1.311.10.12.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ANY_CERT_POLICY_OID = "2.5.29.32.0";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_AUTHORITY_KEY_IDENTIFIER_OID = "2.5.29.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_KEY_ATTRIBUTES_OID = "2.5.29.2";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CERT_POLICIES_95_OID = "2.5.29.3";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_KEY_USAGE_RESTRICTION_OID = "2.5.29.4";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_LEGACY_POLICY_MAPPINGS_OID = "2.5.29.5";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_SUBJECT_ALT_NAME_OID = "2.5.29.7";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ISSUER_ALT_NAME_OID = "2.5.29.8";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_BASIC_CONSTRAINTS_OID = "2.5.29.10";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_SUBJECT_KEY_IDENTIFIER_OID = "2.5.29.14";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_KEY_USAGE_OID = "2.5.29.15";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PRIVATEKEY_USAGE_PERIOD_OID = "2.5.29.16";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_SUBJECT_ALT_NAME2_OID = "2.5.29.17";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ISSUER_ALT_NAME2_OID = "2.5.29.18";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_BASIC_CONSTRAINTS2_OID = "2.5.29.19";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_NAME_CONSTRAINTS_OID = "2.5.29.30";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CRL_DIST_POINTS_OID = "2.5.29.31";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CERT_POLICIES_OID = "2.5.29.32";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_POLICY_MAPPINGS_OID = "2.5.29.33";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_AUTHORITY_KEY_IDENTIFIER2_OID = "2.5.29.35";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_POLICY_CONSTRAINTS_OID = "2.5.29.36";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_ENHANCED_KEY_USAGE_OID = "2.5.29.37";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_CERTIFICATE_TEMPLATE_OID = "1.3.6.1.4.1.311.21.7";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_APPLICATION_CERT_POLICIES_OID = "1.3.6.1.4.1.311.21.10";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_APPLICATION_POLICY_MAPPINGS_OID = "1.3.6.1.4.1.311.21.11";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_APPLICATION_POLICY_CONSTRAINTS_OID = "1.3.6.1.4.1.311.21.12";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_AUTHORITY_INFO_ACCESS_OID = "1.3.6.1.5.5.7.1.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PKIX_POLICY_QUALIFIER_CPS_OID = "1.3.6.1.5.5.7.2.1";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PKIX_POLICY_QUALIFIER_USERNOTICE_OID = "1.3.6.1.5.5.7.2.2";

        public const Int32 CAPICOM_TRUST_IS_NOT_TIME_VALID = 1;
        public const Int32 CAPICOM_TRUST_IS_NOT_TIME_NESTED = 2;
        public const Int32 CAPICOM_TRUST_IS_REVOKED = 4;
        public const Int32 CAPICOM_TRUST_IS_NOT_SIGNATURE_VALID = 8;
        public const Int32 CAPICOM_TRUST_IS_NOT_VALID_FOR_USAGE = 16;
        public const Int32 CAPICOM_TRUST_IS_UNTRUSTED_ROOT = 32;
        public const Int32 CAPICOM_TRUST_REVOCATION_STATUS_UNKNOWN = 64;
        public const Int32 CAPICOM_TRUST_IS_CYCLIC = 128;
        public const Int32 CAPICOM_TRUST_INVALID_EXTENSION = 256;
        public const Int32 CAPICOM_TRUST_INVALID_POLICY_CONSTRAINTS = 512;
        public const Int32 CAPICOM_TRUST_INVALID_BASIC_CONSTRAINTS = 1024;
        public const Int32 CAPICOM_TRUST_INVALID_NAME_CONSTRAINTS = 2048;
        public const Int32 CAPICOM_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT = 4096;
        public const Int32 CAPICOM_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT = 8192;
        public const Int32 CAPICOM_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT = 16384;
        public const Int32 CAPICOM_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT = 32768;
        public const Int32 CAPICOM_TRUST_IS_OFFLINE_REVOCATION = 16777216;
        public const Int32 CAPICOM_TRUST_NO_ISSUANCE_CHAIN_POLICY = 33554432;
        public const Int32 CAPICOM_TRUST_IS_PARTIAL_CHAIN = 65536;
        public const Int32 CAPICOM_TRUST_CTL_IS_NOT_TIME_VALID = 131072;
        public const Int32 CAPICOM_TRUST_CTL_IS_NOT_SIGNATURE_VALID = 262144;
        public const Int32 CAPICOM_TRUST_CTL_IS_NOT_VALID_FOR_USAGE = 524288;

        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_DEF_PROV = "Microsoft Base Cryptographic Provider v1.0";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_ENHANCED_PROV = "Microsoft Enhanced Cryptographic Provider v1.0";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_STRONG_PROV = "Microsoft Strong Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_DEF_RSA_SIG_PROV = "Microsoft RSA Signature Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_DEF_RSA_SCHANNEL_PROV = "Microsoft RSA SChannel Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_DEF_DSS_PROV = "Microsoft Base DSS Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_DEF_DSS_DH_PROV = "Microsoft Base DSS and Diffie-Hellman Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_ENH_DSS_DH_PROV = "Microsoft Enhanced DSS and Diffie-Hellman Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_DEF_DH_SCHANNEL_PROV = "Microsoft DH SChannel Cryptographic Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_SCARD_PROV = "Microsoft Base Smart Card Crypto Provider";
        [MarshalAs(UnmanagedType.LPStr)] public const String CAPICOM_PROV_MS_ENH_RSA_AES_PROV = "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)";
        }
    }
