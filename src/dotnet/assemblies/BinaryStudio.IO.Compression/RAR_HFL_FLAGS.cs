using System;

namespace BinaryStudio.IO.Compression
    {
    [Flags]
    public enum RAR_HFL_FLAGS
        {
        HFL_EXTRA           = 0x0001,
        HFL_DATA            = 0x0002,
        HFL_SKIPIFUNKNOWN   = 0x0004,
        HFL_SPLITBEFORE     = 0x0008,
        HFL_SPLITAFTER      = 0x0010,
        HFL_CHILD           = 0x0020,
        HFL_INHERITED       = 0x0040
        }
    }