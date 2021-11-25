using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_VALUE
        {
        public unsafe fixed Byte RawBytes[24];
        public UInt32 TailOfRawBytes;
        public UInt32 Type;
        }
    }