using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    internal struct CERT_SYSTEM_STORE_RELOCATE_PARA
        {
        public readonly IntPtr KeyBase;
        public readonly IntPtr SystemStore;
        }
    }