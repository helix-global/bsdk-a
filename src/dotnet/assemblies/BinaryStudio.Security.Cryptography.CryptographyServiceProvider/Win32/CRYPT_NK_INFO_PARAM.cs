using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    internal struct CRYPT_NK_INFO_PARAM
        {
        public Int16 N;
        public Int16 K;
        public Byte Part;
        }
    }