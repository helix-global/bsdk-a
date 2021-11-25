using System;

namespace BinaryStudio.PortableExecutable
    {
    [Flags]
    public enum ImageFlags
        {
        None,
        Is64Bit = 1
        }
    }