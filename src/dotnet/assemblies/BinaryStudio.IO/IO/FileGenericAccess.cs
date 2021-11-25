using System;

namespace BinaryStudio.IO
    {
    [Flags]
    internal enum FileGenericAccess : uint
        {
        Read    = 0x80000000,
        Write   = 0x40000000,
        Execute = 0x20000000,
        All     = 0x10000000
        }
    }
