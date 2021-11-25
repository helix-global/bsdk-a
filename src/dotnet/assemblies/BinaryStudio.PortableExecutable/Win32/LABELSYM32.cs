using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LABELSYM32
        {
        public readonly DEBUG_TYPE_ENUM TypeIndex;
        public readonly UInt32 Offset;
        public readonly UInt16 Segment;
        public readonly CV_PFLAG Flags;
        }
    }