using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_REVOCATION_INFO
        {
        public readonly Int32                       Size;
        public readonly CertificateChainErrorStatus RevocationResult;
        public readonly unsafe Byte*                RevocationOid;
        public readonly IntPtr                      OidSpecificInfo;
        public readonly Boolean                     HasFreshnessTime;
        public readonly DWORD                       FreshnessTime;
        public readonly unsafe CERT_REVOCATION_CRL_INFO*   CrlInfo;
        }
    }