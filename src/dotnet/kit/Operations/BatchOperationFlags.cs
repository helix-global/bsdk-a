using System;

namespace Operations
    {
    [Flags]
    public enum BatchOperationFlags
        {
        None,
        Rename    = 0x001,
        Serialize = 0x002,
        Extract   = 0x004,
        Group     = 0x008,
        Install   = 0x010,
        Uninstall = 0x020,
        AbstractSyntaxNotation = 0x040,
        Verify    = 0x080,
        Report    = 0x100,
        Force     = 0x200
        }
    }