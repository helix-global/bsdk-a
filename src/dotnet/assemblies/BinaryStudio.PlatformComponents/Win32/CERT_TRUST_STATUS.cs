using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_TRUST_STATUS
        {
        public CertificateChainErrorStatus ErrorStatus;
        public CertificateChainInfoStatus  InfoStatus;
        }
    }