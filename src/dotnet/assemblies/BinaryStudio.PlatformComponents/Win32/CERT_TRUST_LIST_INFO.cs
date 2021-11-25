using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_TRUST_LIST_INFO
        {
        DWORD         cbSize;
        readonly unsafe CTL_ENTRY*    pCtlEntry;
        readonly unsafe CTL_CONTEXT* pCtlContext;
        }
    }