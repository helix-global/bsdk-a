using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.IO
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct LargeInteger
        {
        [FieldOffset(0)] public Int32  LowPart;
        [FieldOffset(4)] public Int32  HighPart;
        [FieldOffset(0)] public Int64  QuadPart;
        [FieldOffset(0)] public UInt32 ULowPart;
        [FieldOffset(4)] public UInt32 UHighPart;
        [FieldOffset(0)] public UInt64 UQuadPart;
        }
    }
