using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BLOCKSYM32
        {
        public readonly UInt32 pParent;    // pointer to the parent
        public readonly UInt32 pEnd;       // pointer to this blocks end
        public readonly UInt32 len;        // Block length
        public readonly UInt32 off;        // Offset in code segment
        public readonly UInt16 seg;        // segment of label
        }
    }