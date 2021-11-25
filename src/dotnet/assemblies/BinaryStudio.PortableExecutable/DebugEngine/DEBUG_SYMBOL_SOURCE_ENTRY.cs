using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_SYMBOL_SOURCE_ENTRY
        {
        public UInt64 ModuleBase;
        public UInt64 Offset;
        public UInt64 FileNameId;
        public UInt64 EngineInternal;
        public UInt32 Size;
        public UInt32 Flags;
        public UInt32 FileNameSize;
        public UInt32 StartLine;
        public UInt32 EndLine;
        public UInt32 StartColumn;
        public UInt32 EndColumn;
        public UInt32 Reserved;
        }
    }