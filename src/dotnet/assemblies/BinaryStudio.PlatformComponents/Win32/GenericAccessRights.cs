using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum GenericAccessRights
        {
        GenericRead    = unchecked((Int32)0x80000000),
        GenericWrite   = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll     = 0x10000000
        }
    }