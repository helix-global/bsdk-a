using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IMAGE_AUX_SYMBOL
        {
        [FieldOffset( 0)] public readonly Int32 TagIndex;
        [FieldOffset( 4)] public readonly Int32 TotalSize;
        [FieldOffset( 8)] public readonly Int32 PointerToLineNumber;
        [FieldOffset(12)] public readonly Int32 PointerToNextFunction;
        [FieldOffset(16)] public readonly Int16 Index;
        }
    }