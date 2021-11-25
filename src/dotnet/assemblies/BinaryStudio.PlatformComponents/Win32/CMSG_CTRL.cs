using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Win32
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1712:Do not prefix enum values with type name", Justification = "<Pending>")]
    #endif
    public enum CMSG_CTRL : uint
        {
        CMSG_CTRL_VERIFY_SIGNATURE       =  1,
        CMSG_CTRL_DECRYPT                =  2,
        CMSG_CTRL_VERIFY_HASH            =  5,
        CMSG_CTRL_ADD_SIGNER             =  6,
        CMSG_CTRL_DEL_SIGNER             =  7,
        CMSG_CTRL_ADD_SIGNER_UNAUTH_ATTR =  8,
        CMSG_CTRL_DEL_SIGNER_UNAUTH_ATTR =  9,
        CMSG_CTRL_ADD_CERT               = 10,
        CMSG_CTRL_DEL_CERT               = 11,
        CMSG_CTRL_ADD_CRL                = 12,
        CMSG_CTRL_DEL_CRL                = 13,
        CMSG_CTRL_ADD_ATTR_CERT          = 14,
        CMSG_CTRL_DEL_ATTR_CERT          = 15,
        CMSG_CTRL_KEY_TRANS_DECRYPT      = 16,
        CMSG_CTRL_KEY_AGREE_DECRYPT      = 17,
        CMSG_CTRL_MAIL_LIST_DECRYPT      = 18,
        CMSG_CTRL_VERIFY_SIGNATURE_EX    = 19,
        CMSG_CTRL_ADD_CMS_SIGNER_INFO    = 20
        }
    }