using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct THUNKSYM32
        {
        public readonly UInt32 pParent;
        public readonly UInt32 pEnd;
        public readonly UInt32 pNext;
        public readonly UInt32 off;
        public readonly UInt16 seg;
        public readonly UInt16 len;
        public readonly THUNK_ORDINAL ord;
        }
    }