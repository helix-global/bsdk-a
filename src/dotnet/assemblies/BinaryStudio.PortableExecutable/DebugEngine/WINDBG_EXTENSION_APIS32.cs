using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct WINDBG_EXTENSION_APIS32
        {
        public UInt32 NotSupported;
        }
    }