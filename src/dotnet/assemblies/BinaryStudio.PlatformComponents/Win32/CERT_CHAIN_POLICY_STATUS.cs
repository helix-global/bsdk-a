using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct CERT_CHAIN_POLICY_STATUS
        {
        public Int32 Size;
        public Int32 Error;
        public Int32 ChainIndex;
        public Int32 ElementIndex;
        public IntPtr ExtraPolicyStatus;
        }
    }