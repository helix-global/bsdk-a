using System;

namespace BinaryStudio.IO.Compression
    {
    [Flags]
    internal enum RAR_FHEXTRA_HTIME_FLAGS
        {
        FHEXTRA_HTIME_UNIXTIME = 0x01, // Use Unix time_t format.
        FHEXTRA_HTIME_MTIME    = 0x02, // mtime is present.
        FHEXTRA_HTIME_CTIME    = 0x04, // ctime is present.
        FHEXTRA_HTIME_ATIME    = 0x08, // atime is present.
        FHEXTRA_HTIME_UNIX_NS  = 0x10  // Unix format with nanosecond precision.
        }
    }