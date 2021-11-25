using System;

namespace BinaryStudio.PortableExecutable
    {
    [Flags]
    public enum FileDumperFlags
        {
        NONE            = 0x0000,
        ARCHIVEMEMBERS  = 0x0001,
        CLRHEADER       = 0x0002,
        DEPENDENTS      = 0x0004,
        DIRECTIVES      = 0x0008,
        EXPORTS         = 0x0010,
        FPO             = 0x0020,
        HEADERS         = 0x0040,
        IMPORTS         = 0x0080,
        LINENUMBERS     = 0x0100,
        PDATA           = 0x0200,
        RELOCATIONS     = 0x0400,
        SUMMARY         = 0x0800,
        SYMBOLS         = 0x1000,
        TLS             = 0x2000,
        UNWINDINFO      = 0x4000,
        ALL             =
            ARCHIVEMEMBERS  |
            CLRHEADER       |
            DEPENDENTS      |
            DIRECTIVES      |
            EXPORTS         |
            FPO             |
            HEADERS         |
            IMPORTS         |
            LINENUMBERS     |
            PDATA           |
            RELOCATIONS     |
            SUMMARY         |
            SYMBOLS         |
            TLS             |
            UNWINDINFO
        }
    }