using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;
    using CRYPT_INTEGER_BLOB = CRYPT_BLOB;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRL_ENTRY
        {
        public readonly CRYPT_INTEGER_BLOB      SerialNumber;
        public readonly FILETIME                RevocationDate;
        public readonly DWORD                   cExtension;
        public readonly unsafe CERT_EXTENSION*  rgExtension;
        }
    }