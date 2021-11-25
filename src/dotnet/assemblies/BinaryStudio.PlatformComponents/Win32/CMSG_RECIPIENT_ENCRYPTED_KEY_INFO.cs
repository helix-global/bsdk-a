using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Microsoft.Win32
    {
    using CRYPT_DATA_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential)]
    public struct CMSG_RECIPIENT_ENCRYPTED_KEY_INFO32
        {
        CERT_ID32                   RecipientId;
        CRYPT_DATA_BLOB             EncryptedKey;
        FILETIME                    Date;
        unsafe CRYPT_ATTRIBUTE_TYPE_VALUE* pOtherAttr;
        }

    [StructLayout(LayoutKind.Sequential)]
    public struct CMSG_RECIPIENT_ENCRYPTED_KEY_INFO64
        {
        CERT_ID64                   RecipientId;
        CRYPT_DATA_BLOB             EncryptedKey;
        FILETIME                    Date;
        unsafe CRYPT_ATTRIBUTE_TYPE_VALUE* pOtherAttr;
        }
    }