using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct OBJNAMESYM
        {
        public readonly UInt32 Signature;
        }
    }