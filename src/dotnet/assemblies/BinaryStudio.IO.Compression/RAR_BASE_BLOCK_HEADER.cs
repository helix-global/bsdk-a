using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.IO.Compression
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RAR_BASE_BLOCK_HEADER
        {
        public readonly UInt16 Crc;
        public readonly RAR_BLOCK_HEADER_TYPE Type;
        public readonly Int16 Flags;
        public readonly Int16 Size;
        }
    }