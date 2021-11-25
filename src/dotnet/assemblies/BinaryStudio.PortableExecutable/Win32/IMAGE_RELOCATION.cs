using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IMAGE_RELOCATION
        {
        [FieldOffset(0)] public Int32 VirtualAddress;
        [FieldOffset(0)] public Int32 RelocCount;
        [FieldOffset(4)] public Int32 SymbolTableIndex;
        [FieldOffset(8)] public Int16 Type;
        }
    }