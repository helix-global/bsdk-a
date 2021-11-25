using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CV_Column_t
        {
        public readonly UInt16 offColumnStart;
        public readonly UInt16 offColumnEnd;
        }
    }