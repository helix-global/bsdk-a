using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CMSG_CTRL_DECRYPT_PARA
        {
        public Int32   Size;
        public IntPtr  CryptProv;
        public KEY_SPEC_TYPE KeySpec;
        public Int32   RecipientIndex;
        }
    }