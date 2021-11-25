using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_PIN_INFO_SOURCE
        {
        public CRYPT_PASSWD_INFO_PARAM Password;
        public CRYPT_NK_INFO_PARAM Info;
        }
    }