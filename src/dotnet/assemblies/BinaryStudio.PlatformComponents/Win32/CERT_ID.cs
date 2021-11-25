using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_HASH_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct CERT_ID32
        {
        [FieldOffset(0)] public readonly CERT_ID_CHOICE IdChoice;
        [FieldOffset(4)] public CERT_ISSUER_SERIAL_NUMBER IssuerSerialNumber;
        [FieldOffset(4)] public readonly CRYPT_HASH_BLOB KeyId;
        [FieldOffset(4)] public readonly CRYPT_HASH_BLOB HashId;
        }

    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct CERT_ID64
        {
        [FieldOffset(0)] public readonly CERT_ID_CHOICE IdChoice;
        [FieldOffset(8)] public CERT_ISSUER_SERIAL_NUMBER IssuerSerialNumber;
        [FieldOffset(8)] public readonly CRYPT_HASH_BLOB KeyId;
        [FieldOffset(8)] public readonly CRYPT_HASH_BLOB HashId;
        }
    }