using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_PIN_INFO
        {
        public CRYPT_PIN_TYPE Type;
        public CRYPT_PIN_INFO_SOURCE Info;
        }
    }