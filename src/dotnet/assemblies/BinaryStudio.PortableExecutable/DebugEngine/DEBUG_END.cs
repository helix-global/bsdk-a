namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    public enum DEBUG_END
        {
        DEBUG_END_PASSIVE          = 0x00000000,
        DEBUG_END_ACTIVE_TERMINATE = 0x00000001,
        DEBUG_END_ACTIVE_DETACH    = 0x00000002,
        DEBUG_END_REENTRANT        = 0x00000003,
        DEBUG_END_DISCONNECT       = 0x00000004
        }
    }