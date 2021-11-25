using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_DISASM
        {
        DEBUG_DISASM_EFFECTIVE_ADDRESS  = 0x00000001,
        DEBUG_DISASM_MATCHING_SYMBOLS   = 0x00000002,
        DEBUG_DISASM_SOURCE_LINE_NUMBER = 0x00000004,
        DEBUG_DISASM_SOURCE_FILE_NAME   = 0x00000008
        }
    }