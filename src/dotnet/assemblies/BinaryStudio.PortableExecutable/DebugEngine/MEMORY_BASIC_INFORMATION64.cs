using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MEMORY_BASIC_INFORMATION64
        {
        public UInt64 BaseAddress;
        public UInt64 AllocationBase;
        public UInt32 AllocationProtect;
        public UInt32 __alignment1;
        public UInt64 RegionSize;
        public UInt32 State;
        public UInt32 Protect;
        public UInt32 Type;
        public UInt32 __alignment2;
        }
    }