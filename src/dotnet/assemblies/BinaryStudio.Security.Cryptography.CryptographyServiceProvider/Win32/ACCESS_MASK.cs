using System;

namespace BinaryStudio.Security.Cryptography.Win32
    {
    [Flags]
    public enum ACCESS_MASK
        {
        GENERIC_READ                     = unchecked((Int32)0x80000000),
        GENERIC_WRITE                    = 0x40000000,
        GENERIC_EXECUTE                  = 0x20000000,
        GENERIC_ALL                      = 0x10000000,
        MAXIMUM_ALLOWED                  = 0x02000000,
        ACCESS_SYSTEM_SECURITY           = 0x01000000,
        DELETE                           = 0x00010000,
        READ_CONTROL                     = 0x00020000,
        WRITE_DAC                        = 0x00040000,
        WRITE_OWNER                      = 0x00080000,
        SYNCHRONIZE                      = 0x00100000
        }
    }