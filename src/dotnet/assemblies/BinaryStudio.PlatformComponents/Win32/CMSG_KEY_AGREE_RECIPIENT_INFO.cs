using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_DATA_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Explicit)]
    public struct CMSG_KEY_AGREE_RECIPIENT_INFO32
        {
        [FieldOffset(0)] public readonly Int32 Version;
        [FieldOffset(4)] public readonly Int32 OriginatorChoice;
        [FieldOffset(8)] public readonly CERT_ID32 OriginatorCertId;
        [FieldOffset(8)] public readonly CERT_PUBLIC_KEY_INFO OriginatorPublicKeyInfo;
        [FieldOffset(8+24)]        public readonly CRYPT_DATA_BLOB             UserKeyingMaterial;
        [FieldOffset(8+24+8)]      public readonly CRYPT_ALGORITHM_IDENTIFIER  KeyEncryptionAlgorithm;
        [FieldOffset(8+24+8+12)]   public readonly Int32 RecipientEncryptedKeyCount;
        [FieldOffset(8+24+8+12+4)] public readonly unsafe CMSG_RECIPIENT_ENCRYPTED_KEY_INFO32* RecipientEncryptedKeys;
        }

    [StructLayout(LayoutKind.Explicit)]
    public struct CMSG_KEY_AGREE_RECIPIENT_INFO64
        {
        [FieldOffset(0)] public readonly Int32 Version;
        [FieldOffset(4)] public readonly Int32 OriginatorChoice;
        [FieldOffset(8)] public readonly CERT_ID32 OriginatorCertId;
        [FieldOffset(8)] public readonly CERT_PUBLIC_KEY_INFO OriginatorPublicKeyInfo;
        [FieldOffset(8+48)] public readonly CRYPT_DATA_BLOB             UserKeyingMaterial;
        [FieldOffset(8+48+16)] public readonly CRYPT_ALGORITHM_IDENTIFIER  KeyEncryptionAlgorithm;
        [FieldOffset(8+48+16+24)] public readonly Int32 RecipientEncryptedKeyCount;
        [FieldOffset(8+48+16+24+4)] public readonly unsafe CMSG_RECIPIENT_ENCRYPTED_KEY_INFO64* RecipientEncryptedKeys;
        }
    }