using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    using SHA_LONG=UInt32;
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHA256_CTX
        {
        private const Int32 SHA_LBLOCK = 16;
        public unsafe fixed SHA_LONG h[8];
        public readonly SHA_LONG Nl, Nh;
        public unsafe fixed SHA_LONG data[SHA_LBLOCK];
        public readonly SHA_LONG num, md_len;
        }
    }