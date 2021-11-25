using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_PASSWD_INFO_PARAM
        {
        public Int32 MinPasswordLength;
        public Int32 MaxPasswordLength;
        public Int32 PasswordType;
        }
    }