using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_OUTPUT
        {
        DEBUG_OUTPUT_NORMAL            = 0x00000001,
        DEBUG_OUTPUT_ERROR             = 0x00000002,
        DEBUG_OUTPUT_WARNING           = 0x00000004,
        DEBUG_OUTPUT_VERBOSE           = 0x00000008,
        DEBUG_OUTPUT_PROMPT            = 0x00000010,
        DEBUG_OUTPUT_PROMPT_REGISTERS  = 0x00000020,
        DEBUG_OUTPUT_EXTENSION_WARNING = 0x00000040,
        DEBUG_OUTPUT_DEBUGGEE          = 0x00000080,
        DEBUG_OUTPUT_DEBUGGEE_PROMPT   = 0x00000100,
        DEBUG_OUTPUT_SYMBOLS           = 0x00000200,
        DEBUG_OUTPUT_STATUS            = 0x00000400,
        DEBUG_OUTPUT_XML               = 0x00000800
        }
    }