using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    using SHA_LONG=UInt32;
    using SHA_LONG64=UInt64;
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHA512_CTX
        {
        private const Int32 SHA_LBLOCK = 16;
        public unsafe fixed SHA_LONG64 h[8];
        public readonly SHA_LONG64 Nl, Nh;
        public unsafe fixed SHA_LONG64 data[SHA_LBLOCK];
        public readonly SHA_LONG num, md_len;
        }
    }