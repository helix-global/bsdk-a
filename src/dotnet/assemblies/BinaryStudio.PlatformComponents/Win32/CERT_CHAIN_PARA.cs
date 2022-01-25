using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_CHAIN_PARA
        {
        public Int32 Size;
        public CERT_USAGE_MATCH RequestedUsage;
        #if CERT_CHAIN_PARA_HAS_EXTRA_FIELDS
        public CERT_USAGE_MATCH RequestedIssuancePolicy;
        public UInt32 UrlRetrievalTimeout;
        public Boolean CheckRevocationFreshnessTime;
        public Int32 RevocationFreshnessTime;
        public unsafe FILETIME* CacheResync;
        #endif
        }
    }