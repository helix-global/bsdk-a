using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_DATA_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CMSG_KEY_TRANS_RECIPIENT_INFO32
        {
        public readonly CMSG_KEY_RECIPIENT_VERSION Version;
        public readonly CERT_ID32 RecipientId;
        public readonly CRYPT_ALGORITHM_IDENTIFIER  KeyEncryptionAlgorithm;
        public readonly CRYPT_DATA_BLOB             EncryptedKey;
        }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CMSG_KEY_TRANS_RECIPIENT_INFO64
        {
        public readonly CMSG_KEY_RECIPIENT_VERSION Version;
        public readonly CERT_ID64 RecipientId;
        public readonly CRYPT_ALGORITHM_IDENTIFIER  KeyEncryptionAlgorithm;
        public readonly CRYPT_DATA_BLOB             EncryptedKey;
        }
    }