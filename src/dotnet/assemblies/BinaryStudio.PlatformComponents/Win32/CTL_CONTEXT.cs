using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct CTL_CONTEXT
        {
        public readonly DWORD dwMsgAndCertEncodingType;
        public readonly unsafe Byte* pbCtlEncoded;
        public readonly DWORD cbCtlEncoded;
        public readonly unsafe CTL_INFO* pCtlInfo;
        public readonly IntPtr hCertStore;
        public readonly IntPtr hCryptMsg;
        public readonly unsafe Byte* pbCtlContent;
        public readonly DWORD cbCtlContent;
        }
    }