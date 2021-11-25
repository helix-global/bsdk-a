using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_CHAIN_CONTEXT
        {
        public Int32 cbSize;
        public CERT_TRUST_STATUS TrustStatus;
        public Int32 ChainCount;
        public unsafe CERT_SIMPLE_CHAIN** ChainArray;
        public DWORD cLowerQualityChainContext;
        public unsafe CERT_CHAIN_CONTEXT** rgpLowerQualityChainContext;
        public Boolean fHasRevocationFreshnessTime;
        public DWORD dwRevocationFreshnessTime;
        public DWORD dwCreateFlags;
        public Guid ChainId;
        }
    }