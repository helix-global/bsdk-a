using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_CHAIN_ELEMENT
        {
        public readonly Int32 Size;
        public readonly unsafe CERT_CONTEXT* CertContext;
        public readonly CERT_TRUST_STATUS TrustStatus;
        public readonly unsafe CERT_REVOCATION_INFO* RevocationInfo;
        public readonly unsafe CERT_ENHKEY_USAGE* IssuanceUsage;
        public readonly unsafe CERT_ENHKEY_USAGE* ApplicationUsage;
        public readonly unsafe Char* ExtendedErrorInfo;
        }
    }