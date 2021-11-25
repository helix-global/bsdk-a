using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CRYPT_OID_GROUP_ID
        {
        CRYPT_HASH_ALG_OID_GROUP_ID     = 1,
        CRYPT_ENCRYPT_ALG_OID_GROUP_ID  = 2,
        CRYPT_PUBKEY_ALG_OID_GROUP_ID   = 3,
        CRYPT_SIGN_ALG_OID_GROUP_ID     = 4,
        CRYPT_RDN_ATTR_OID_GROUP_ID     = 5,
        CRYPT_EXT_OR_ATTR_OID_GROUP_ID  = 6,
        CRYPT_ENHKEY_USAGE_OID_GROUP_ID = 7,
        CRYPT_POLICY_OID_GROUP_ID       = 8,
        CRYPT_TEMPLATE_OID_GROUP_ID     = 9,
        CRYPT_KDF_OID_GROUP_ID          = 10,
        CRYPT_LAST_OID_GROUP_ID         = 10,
        CRYPT_FIRST_ALG_OID_GROUP_ID    = CRYPT_HASH_ALG_OID_GROUP_ID,
        CRYPT_LAST_ALG_OID_GROUP_ID     = CRYPT_SIGN_ALG_OID_GROUP_ID,
        CRYPT_OID_DISABLE_SEARCH_DS_FLAG = unchecked((Int32)0x80000000)
        }
    }