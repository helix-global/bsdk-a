using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRYPT_BLOB
        {
        public Int32 Size;
        public unsafe Byte* Data;
        }
    }