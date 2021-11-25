using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_PROCESS
        {
        DEBUG_PROCESS_DETACH_ON_EXIT    = 0x00000001,
        DEBUG_PROCESS_ONLY_THIS_PROCESS = 0x00000002
        }
    }