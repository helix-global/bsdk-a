using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_CREATE_PROCESS
        {
        DEBUG_CREATE_PROCESS_NO_DEBUG_HEAP = 0x00000400,
        DEBUG_CREATE_PROCESS_THROUGH_RTL   = 0x00010000,
        }
    }