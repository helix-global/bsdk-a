using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_GET_PROC
        {
        DEBUG_GET_PROC_DEFAULT      = 0x00000000,
        DEBUG_GET_PROC_FULL_MATCH   = 0x00000001,
        DEBUG_GET_PROC_ONLY_MATCH   = 0x00000002,
        DEBUG_GET_PROC_SERVICE_NAME = 0x00000004
        }
    }