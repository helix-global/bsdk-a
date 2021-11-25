using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using HCRYPTPROV_LEGACY = IntPtr;
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CRYPT_ENCRYPT_MESSAGE_PARA
        {
        public Int32                       Size;
        public CRYPT_MSG_TYPE         MsgEncodingType;
        public HCRYPTPROV_LEGACY           CryptProv;
        public CRYPT_ALGORITHM_IDENTIFIER  ContentEncryptionAlgorithm;
        public unsafe void                 *EncryptionAuxInfo;
        public CRYPT_MESSAGE_FLAGS         Flags;
        public CMSG_TYPE                   InnerContentType;
        }
    }