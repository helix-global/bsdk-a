using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;
    using CRYPT_ATTR_BLOB = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRYPT_ATTRIBUTE
        {
        public readonly unsafe Byte*            pszObjId;
        public readonly DWORD                   cValue;
        public readonly unsafe CRYPT_ATTR_BLOB* rgValue;
        }
    }