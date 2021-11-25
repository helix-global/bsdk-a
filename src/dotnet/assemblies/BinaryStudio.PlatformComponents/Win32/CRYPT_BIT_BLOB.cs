using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRYPT_BIT_BLOB
        {
        public readonly UInt32 cbData;
        public readonly IntPtr pbData;
        public readonly UInt32 cUnusedBits;
        }
    }