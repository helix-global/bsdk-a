using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEBUG_SYMBOL_HEADER
        {
        public readonly UInt16 Length;
        public readonly DEBUG_SYMBOL_INDEX Type;
        }
    }