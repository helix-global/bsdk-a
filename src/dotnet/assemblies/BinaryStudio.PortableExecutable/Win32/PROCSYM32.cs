using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PROCSYM32
        {
        public readonly UInt32 Parent;
        public readonly UInt32 End;
        public readonly UInt32 Next;
        public readonly UInt32 Length;
        public readonly UInt32 DbgStart;
        public readonly UInt32 DbgEnd;
        public readonly DEBUG_TYPE_ENUM TypeIndex;
        public readonly UInt32 Offset;
        public readonly UInt16 Segment;
        public readonly CV_PFLAG Flags;
        }
    }