using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_ATTACH
        {
        DEBUG_ATTACH_DEFAULT                   = 0x00000000,
        DEBUG_ATTACH_NONINVASIVE               = 0x00000001,
        DEBUG_ATTACH_EXISTING                  = 0x00000002,
        DEBUG_ATTACH_NONINVASIVE_NO_SUSPEND    = 0x00000004,
        DEBUG_ATTACH_INVASIVE_NO_INITIAL_BREAK = 0x00000008,
        DEBUG_ATTACH_INVASIVE_RESUME_PROCESS   = 0x00000010,
        DEBUG_ATTACH_NONINVASIVE_ALLOW_PARTIAL = 0x00000020
        }
    }