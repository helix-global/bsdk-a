using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct CRYPT_DECRYPT_MESSAGE_PARA
        {
        public Int32 Size;
        public CRYPT_MSG_TYPE MsgAndCertEncodingType;
        public Int32 CertStoreCount;
        public unsafe IntPtr* CertStore;
        #if CRYPT_DECRYPT_MESSAGE_PARA_HAS_EXTRA_FIELDS
        public UInt32 Flags;
        #endif
        }
    }