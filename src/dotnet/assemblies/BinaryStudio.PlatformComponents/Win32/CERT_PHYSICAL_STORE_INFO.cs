using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CRYPT_DATA_BLOB = CRYPT_BLOB;
    [StructLayout(LayoutKind.Sequential)]
    public struct CERT_PHYSICAL_STORE_INFO
        {
        public readonly Int32           Size;
        public readonly unsafe Byte*    OpenStoreProvider;
        public readonly Int32           OpenEncodingType;
        public readonly CERT_SYSTEM_STORE_FLAGS OpenFlags;
        public readonly CRYPT_DATA_BLOB OpenParameters;
        public readonly CERT_PHYSICAL_STORE_FLAGS Flags;
        public readonly Int32           Priority;
        }
    }