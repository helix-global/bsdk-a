using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct CERT_SYSTEM_STORE_INFO
        {
        public readonly Int32 Size;
        }
    }