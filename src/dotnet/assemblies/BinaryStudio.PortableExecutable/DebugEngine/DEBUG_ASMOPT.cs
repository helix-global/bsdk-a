using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_ASMOPT
        {
        DEBUG_ASMOPT_DEFAULT             = 0x00000000,
        DEBUG_ASMOPT_VERBOSE             = 0x00000001,
        DEBUG_ASMOPT_NO_CODE_BYTES       = 0x00000002,
        DEBUG_ASMOPT_IGNORE_OUTPUT_WIDTH = 0x00000004,
        DEBUG_ASMOPT_SOURCE_LINE_NUMBER  = 0x00000008
        }
    }