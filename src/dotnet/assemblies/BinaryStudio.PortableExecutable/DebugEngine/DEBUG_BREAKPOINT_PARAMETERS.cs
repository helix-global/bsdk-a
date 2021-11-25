using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_BREAKPOINT_PARAMETERS
        {
        public UInt64 Offset;
        public UInt32 Id;
        public UInt32 BreakType;
        public UInt32 ProcType;
        public UInt32 Flags;
        public UInt32 DataSize;
        public UInt32 DataAccessType;
        public UInt32 PassCount;
        public UInt32 CurrentPassCount;
        public UInt32 MatchThread;
        public UInt32 CommandSize;
        public UInt32 OffsetExpressionSize;
        }
    }