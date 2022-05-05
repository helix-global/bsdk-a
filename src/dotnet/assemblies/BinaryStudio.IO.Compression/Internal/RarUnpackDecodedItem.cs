using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.IO.Compression
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct RarUnpackDecodedItem
        {
        [FieldOffset(0)] public RAR_UNP_DEC_TYPE Type;
        [FieldOffset(4)] public Int16 Size;
        [FieldOffset(6)] public Int32 Distance;
        [FieldOffset(6)] public unsafe fixed Byte Literal[4];
        }
    }