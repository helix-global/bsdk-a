namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum SYMOPT : uint
        {
        SYMOPT_CASE_INSENSITIVE           = 0x00000001,
        SYMOPT_UNDNAME                    = 0x00000002,
        SYMOPT_DEFERRED_LOADS             = 0x00000004,
        SYMOPT_NO_CPP                     = 0x00000008,
        SYMOPT_LOAD_LINES                 = 0x00000010,
        SYMOPT_OMAP_FIND_NEAREST          = 0x00000020,
        SYMOPT_LOAD_ANYTHING              = 0x00000040,
        SYMOPT_IGNORE_CVREC               = 0x00000080,
        SYMOPT_NO_UNQUALIFIED_LOADS       = 0x00000100,
        SYMOPT_FAIL_CRITICAL_ERRORS       = 0x00000200,
        SYMOPT_EXACT_SYMBOLS              = 0x00000400,
        SYMOPT_ALLOW_ABSOLUTE_SYMBOLS     = 0x00000800,
        SYMOPT_IGNORE_NT_SYMPATH          = 0x00001000,
        SYMOPT_INCLUDE_32BIT_MODULES      = 0x00002000,
        SYMOPT_PUBLICS_ONLY               = 0x00004000,
        SYMOPT_NO_PUBLICS                 = 0x00008000,
        SYMOPT_AUTO_PUBLICS               = 0x00010000,
        SYMOPT_NO_IMAGE_SEARCH            = 0x00020000,
        SYMOPT_SECURE                     = 0x00040000,
        SYMOPT_NO_PROMPTS                 = 0x00080000,
        SYMOPT_OVERWRITE                  = 0x00100000,
        SYMOPT_IGNORE_IMAGEDIR            = 0x00200000,
        SYMOPT_FLAT_DIRECTORY             = 0x00400000,
        SYMOPT_FAVOR_COMPRESSED           = 0x00800000,
        SYMOPT_ALLOW_ZERO_ADDRESS         = 0x01000000,
        SYMOPT_DISABLE_SYMSRV_AUTODETECT  = 0x02000000,
        SYMOPT_READONLY_CACHE             = 0x04000000,
        SYMOPT_SYMPATH_LAST               = 0x08000000,
        SYMOPT_DISABLE_FAST_SYMBOLS       = 0x10000000,
        SYMOPT_DISABLE_SYMSRV_TIMEOUT     = 0x20000000,
        SYMOPT_DISABLE_SRVSTAR_ON_STARTUP = 0x40000000,
        SYMOPT_DEBUG                      = 0x80000000
        }
    }