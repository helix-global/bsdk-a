using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CERT_PUBLIC_KEY_INFO
        {
        public CRYPT_ALGORITHM_IDENTIFIER    Algorithm;
        public CRYPT_BIT_BLOB                PublicKey;
        }
    }