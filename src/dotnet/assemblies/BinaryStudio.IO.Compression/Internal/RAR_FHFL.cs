using System;

namespace BinaryStudio.IO.Compression
    {
    [Flags]
    internal enum RAR_FHFL
        {
        FHFL_DIRECTORY      = 0x0001, // Directory.
        FHFL_UTIME          = 0x0002, // Time field in Unix format is present.
        FHFL_CRC32          = 0x0004, // CRC32 field is present.
        FHFL_UNPUNKNOWN     = 0x0008  // Unknown unpacked size.
        }
    }