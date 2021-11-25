using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct DEBUG_MODULE_AND_ID
        {
        public UInt64 ModuleBase;
        public UInt64 Id;
        }
    }