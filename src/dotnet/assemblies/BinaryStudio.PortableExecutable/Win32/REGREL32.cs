using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct REGREL32
        {
        public readonly UInt32 off;
        public readonly DEBUG_TYPE_ENUM TypeIndex;
        public readonly UInt16 reg;
        }
    }