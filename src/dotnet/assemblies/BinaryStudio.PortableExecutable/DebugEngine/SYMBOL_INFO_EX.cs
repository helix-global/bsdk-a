using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SYMBOL_INFO_EX
        {
        public UInt32 SizeOfStruct;
        public UInt32 TypeOfInfo;
        public UInt64 Offset;
        public UInt32 Line;
        public UInt32 Displacement;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public UInt32[] Reserved;
        }
    }