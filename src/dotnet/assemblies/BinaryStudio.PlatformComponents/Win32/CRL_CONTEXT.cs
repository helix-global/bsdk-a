using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRL_CONTEXT
        {
        public readonly DWORD               CertEncodingType;
        public readonly unsafe Byte*        CrlEncodedData;
        public readonly Int32               CrlEncodedSize;
        public readonly unsafe CRL_INFO*    CrlInfo;
        public readonly IntPtr              CertStore;
        }
    }