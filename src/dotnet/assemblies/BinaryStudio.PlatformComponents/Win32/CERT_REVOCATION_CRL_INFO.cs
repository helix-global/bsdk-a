using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_REVOCATION_CRL_INFO
        {
        public readonly Int32 Size;
        public readonly unsafe CRL_CONTEXT* BaseCrlContext;
        public readonly unsafe CRL_CONTEXT* DeltaCrlContext;
        public readonly unsafe CRL_ENTRY* CrlEntry;
        public readonly Boolean DeltaCrlEntry;
        }
    }