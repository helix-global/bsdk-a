using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CERT_CHAIN_FLAGS
        {
        CERT_CHAIN_CACHE_END_CERT                           = unchecked((Int32)0x00000001),
        CERT_CHAIN_THREAD_STORE_SYNC                        = unchecked((Int32)0x00000002),
        CERT_CHAIN_CACHE_ONLY_URL_RETRIEVAL                 = unchecked((Int32)0x00000004),
        CERT_CHAIN_USE_LOCAL_MACHINE_STORE                  = unchecked((Int32)0x00000008),
        CERT_CHAIN_ENABLE_CACHE_AUTO_UPDATE                 = unchecked((Int32)0x00000010),
        CERT_CHAIN_ENABLE_SHARE_STORE                       = unchecked((Int32)0x00000020),
        CERT_CHAIN_REVOCATION_CHECK_END_CERT                = unchecked((Int32)0x10000000),
        CERT_CHAIN_REVOCATION_CHECK_CHAIN                   = unchecked((Int32)0x20000000),
        CERT_CHAIN_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT      = unchecked((Int32)0x40000000),
        CERT_CHAIN_REVOCATION_CHECK_CACHE_ONLY              = unchecked((Int32)0x80000000),
        CERT_CHAIN_REVOCATION_ACCUMULATIVE_TIMEOUT          = unchecked((Int32)0x08000000),
        CERT_CHAIN_REVOCATION_CHECK_OCSP_CERT               = unchecked((Int32)0x04000000),
        CERT_CHAIN_DISABLE_PASS1_QUALITY_FILTERING          = unchecked((Int32)0x00000040),
        CERT_CHAIN_RETURN_LOWER_QUALITY_CONTEXTS            = unchecked((Int32)0x00000080),
        CERT_CHAIN_DISABLE_AUTH_ROOT_AUTO_UPDATE            = unchecked((Int32)0x00000100),
        CERT_CHAIN_TIMESTAMP_TIME                           = unchecked((Int32)0x00000200),
        CERT_CHAIN_ENABLE_PEER_TRUST                        = unchecked((Int32)0x00000400),
        CERT_CHAIN_DISABLE_MY_PEER_TRUST                    = unchecked((Int32)0x00000800),
        CERT_CHAIN_DISABLE_MD2_MD4                          = unchecked((Int32)0x00001000),
        CERT_CHAIN_DISABLE_AIA                              = unchecked((Int32)0x00002000),
        CERT_CHAIN_HAS_MOTW                                 = unchecked((Int32)0x00004000),
        CERT_CHAIN_ONLY_ADDITIONAL_AND_AUTH_ROOT            = unchecked((Int32)0x00008000),
        CERT_CHAIN_OPT_IN_WEAK_SIGNATURE                    = unchecked((Int32)0x00010000),
        CERT_CHAIN_STRONG_SIGN_DISABLE_END_CHECK_FLAG       = unchecked((Int32)0x00000001),
        CERT_CHAIN_EXCLUSIVE_ENABLE_CA_FLAG                 = unchecked((Int32)0x00000001),
        }
    }