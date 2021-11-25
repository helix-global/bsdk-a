namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    public enum DEBUG_OUTCTL : uint
        {
        DEBUG_OUTCTL_THIS_CLIENT       = 0x00000000,
        DEBUG_OUTCTL_ALL_CLIENTS       = 0x00000001,
        DEBUG_OUTCTL_ALL_OTHER_CLIENTS = 0x00000002,
        DEBUG_OUTCTL_IGNORE            = 0x00000003,
        DEBUG_OUTCTL_LOG_ONLY          = 0x00000004,
        DEBUG_OUTCTL_SEND_MASK         = 0x00000007,
        DEBUG_OUTCTL_NOT_LOGGED        = 0x00000008,
        DEBUG_OUTCTL_OVERRIDE_MASK     = 0x00000010,
        DEBUG_OUTCTL_DML               = 0x00000020,
        DEBUG_OUTCTL_AMBIENT_DML       = 0xfffffffe,
        DEBUG_OUTCTL_AMBIENT_TEXT      = 0xffffffff,
        DEBUG_OUTCTL_AMBIENT           = DEBUG_OUTCTL_AMBIENT_TEXT
        }
    }