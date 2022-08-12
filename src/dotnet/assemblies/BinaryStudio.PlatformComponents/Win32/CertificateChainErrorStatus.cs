using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CertificateChainErrorStatus
        {
        CERT_TRUST_NO_ERROR                             = 0x00000000,
        TrustIsNotTimeValid                             = 0x00000001,
        CERT_TRUST_IS_NOT_TIME_NESTED                   = 0x00000002,
        CERT_TRUST_IS_REVOKED                           = 0x00000004,
        CERT_TRUST_IS_NOT_SIGNATURE_VALID               = 0x00000008,
        CERT_TRUST_IS_NOT_VALID_FOR_USAGE               = 0x00000010,
        CERT_TRUST_IS_UNTRUSTED_ROOT                    = 0x00000020,
        CERT_TRUST_REVOCATION_STATUS_UNKNOWN            = 0x00000040,
        CERT_TRUST_IS_CYCLIC                            = 0x00000080,
        CERT_TRUST_INVALID_EXTENSION                    = 0x00000100,
        CERT_TRUST_INVALID_POLICY_CONSTRAINTS           = 0x00000200,
        CERT_TRUST_INVALID_BASIC_CONSTRAINTS            = 0x00000400,
        CERT_TRUST_INVALID_NAME_CONSTRAINTS             = 0x00000800,
        CERT_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT    = 0x00001000,
        CERT_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT      = 0x00002000,
        CERT_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT    = 0x00004000,
        CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT         = 0x00008000,
        CERT_TRUST_IS_OFFLINE_REVOCATION                = 0x01000000,
        CERT_TRUST_NO_ISSUANCE_CHAIN_POLICY             = 0x02000000,
        CERT_TRUST_IS_EXPLICIT_DISTRUST                 = 0x04000000,
        CERT_TRUST_HAS_NOT_SUPPORTED_CRITICAL_EXT       = 0x08000000,
        CERT_TRUST_IS_PARTIAL_CHAIN                     = 0x00010000,
        CERT_TRUST_CTL_IS_NOT_TIME_VALID                = 0x00020000,
        CERT_TRUST_CTL_IS_NOT_SIGNATURE_VALID           = 0x00040000,
        CERT_TRUST_CTL_IS_NOT_VALID_FOR_USAGE           = 0x00080000,
        CERT_TRUST_HAS_WEAK_SIGNATURE                   = 0x00100000,
        CERT_TRUST_HAS_WEAK_HYGIENE                     = 0x00200000
        }
    }