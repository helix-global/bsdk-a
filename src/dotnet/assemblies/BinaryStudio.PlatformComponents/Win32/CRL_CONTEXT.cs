using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRL_CONTEXT
        {
        public readonly DWORD               dwCertEncodingType;
        public readonly unsafe Byte*        pbCrlEncoded;
        public readonly Int32               cbCrlEncoded;
        public readonly unsafe CRL_INFO*    pCrlInfo;
        public readonly IntPtr              hCertStore;
        }
    }