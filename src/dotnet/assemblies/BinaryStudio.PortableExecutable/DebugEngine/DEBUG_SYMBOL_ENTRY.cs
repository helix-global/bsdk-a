using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_SYMBOL_ENTRY
        {
        public UInt64 ModuleBase;
        public UInt64 Offset;
        public UInt64 Id;
        public UInt64 Arg64;
        public UInt32 Size;
        public UInt32 Flags;
        public UInt32 TypeId;
        public UInt32 NameSize;
        public UInt32 Token;
        public UInt32 Tag;
        public UInt32 Arg32;
        public UInt32 Reserved;
        }
    }