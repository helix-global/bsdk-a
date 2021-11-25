using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CV_Line_t
        {
        public readonly UInt32 offset;
        public readonly UInt32 Value;
        }
    }