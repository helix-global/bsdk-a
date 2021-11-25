using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct CRYPT_KEY_VERIFY_MESSAGE_PARA
        {
        public Int32 Size;
        public CRYPT_MSG_TYPE MsgEncodingType;
        public IntPtr CryptProv;
        }
    }