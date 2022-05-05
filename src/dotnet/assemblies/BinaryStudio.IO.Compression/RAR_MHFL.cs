using System;

namespace BinaryStudio.IO.Compression
    {
    [Flags]
    internal enum RAR_MHFL
        {
        MHFL_VOLUME         = 0x0001, // Volume.
        MHFL_VOLNUMBER      = 0x0002, // Volume number field is present. True for all volumes except first.
        MHFL_SOLID          = 0x0004, // Solid archive.
        MHFL_PROTECT        = 0x0008, // Recovery record is present.
        MHFL_LOCK           = 0x0010  // Locked archive.
        }
    }