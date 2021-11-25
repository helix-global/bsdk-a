using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DEBUG_SSECTION_HEADER
        {
        public readonly DEBUG_S Type;
        public readonly Int32 Length;
        }
    }