using System;

namespace Operations
    {
    [Flags]
    public enum BatchOperationFlags
        {
        None,
        Rename    = 0x01,
        Serialize = 0x02,
        Extract   = 0x04,
        Group     = 0x08,
        Install   = 0x10,
        Uninstall = 0x20,
        AbstractSyntaxNotation = 0x40
        }
    }