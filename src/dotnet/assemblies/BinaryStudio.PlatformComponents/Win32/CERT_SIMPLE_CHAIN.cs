using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_SIMPLE_CHAIN
        {
        public readonly Int32 Size;
        public readonly CERT_TRUST_STATUS TrustStatus;
        public readonly DWORD ElementCount;
        public readonly unsafe CERT_CHAIN_ELEMENT** ElementArray;
        public readonly unsafe CERT_TRUST_LIST_INFO* TrustListInfo;
        public readonly Boolean HasRevocationFreshnessTime;
        public readonly DWORD RevocationFreshnessTime;
        }
    }