using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CMSG_ENVELOPED_ENCODE_INFO
        {
        public Int32 Size;
        public IntPtr CryptProv;
        public CRYPT_ALGORITHM_IDENTIFIER ContentEncryptionAlgorithm;
        public IntPtr EncryptionAuxInfo;
        public Int32 RecipientsCount;
        public unsafe CERT_INFO** Recipients;
        }
    }