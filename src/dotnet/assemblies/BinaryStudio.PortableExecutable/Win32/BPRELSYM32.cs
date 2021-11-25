using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BPRELSYM32
        {
        public readonly UInt32 Offset;
        public readonly DEBUG_TYPE_ENUM TypeIndex;
        }
    }