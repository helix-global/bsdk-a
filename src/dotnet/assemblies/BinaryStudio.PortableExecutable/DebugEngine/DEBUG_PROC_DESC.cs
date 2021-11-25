using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_PROC_DESC
        {
        DEBUG_PROC_DESC_DEFAULT         = 0x00000000,
        DEBUG_PROC_DESC_NO_PATHS        = 0x00000001,
        DEBUG_PROC_DESC_NO_SERVICES     = 0x00000002,
        DEBUG_PROC_DESC_NO_MTS_PACKAGES = 0x00000004,
        DEBUG_PROC_DESC_NO_COMMAND_LINE = 0x00000008,
        DEBUG_PROC_DESC_NO_SESSION_ID   = 0x00000010,
        DEBUG_PROC_DESC_NO_USER_NAME    = 0x00000020
        }
    }