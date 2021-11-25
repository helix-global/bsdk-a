using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_STACK_FRAME_EX
        {
        public UInt64 InstructionOffset;
        public UInt64 ReturnOffset;
        public UInt64 FrameOffset;
        public UInt64 StackOffset;
        public UInt64 FuncTableEntry;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public UInt64[] Params;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public UInt64[] Reserved;
        public Int32 Virtual;
        public UInt32 FrameNumber;
        public UInt32 InlineFrameContext;
        public UInt32 Reserved1;
        }
    }