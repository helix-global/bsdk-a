using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_REGISTER_DESCRIPTION
        {
        public UInt32 Type;
        public UInt32 Flags;
        public UInt32 SubregMaster;
        public UInt32 SubregLength;
        public UInt64 SubregMask;
        public UInt32 SubregShift;
        public UInt32 Reserved0;
        }
    }