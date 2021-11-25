using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct HEAPALLOCSITE
        {
        public readonly UInt32 off;                // offset of call site
        public readonly UInt16 sect;               // section index of call site
        public readonly UInt16 cbInstr;            // length of heap allocation call instruction
        public readonly DEBUG_TYPE_ENUM TypeIndex; // type index describing function signature
        }
    }