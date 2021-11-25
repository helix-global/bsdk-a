using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CV8_MAJOR_RECORD_HEADER
        {
        public readonly CV8_MAJOR_RECORD_TYPE Type;
        public readonly Int32 Length;
        }
    }