using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_CONNECT_SESSION
        {
        DEBUG_CONNECT_SESSION_DEFAULT     = 0x00000000,
        DEBUG_CONNECT_SESSION_NO_VERSION  = 0x00000001,
        DEBUG_CONNECT_SESSION_NO_ANNOUNCE = 0x00000002
        }
    }