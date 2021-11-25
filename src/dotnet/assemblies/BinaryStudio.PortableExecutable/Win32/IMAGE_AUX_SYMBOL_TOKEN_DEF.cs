using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 18)]
    public struct IMAGE_AUX_SYMBOL_TOKEN_DEF
        {
        [FieldOffset(2)] public readonly Int32 SymbolTableIndex;
        }
    }