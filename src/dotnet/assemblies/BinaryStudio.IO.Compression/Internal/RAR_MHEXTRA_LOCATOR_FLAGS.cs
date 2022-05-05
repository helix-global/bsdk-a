using System;

namespace BinaryStudio.IO.Compression
    {
    [Flags]
    internal enum RAR_MHEXTRA_LOCATOR_FLAGS
        {
        MHEXTRA_LOCATOR_QLIST = 0x01,
        MHEXTRA_LOCATOR_RR    = 0x02
        }
    }