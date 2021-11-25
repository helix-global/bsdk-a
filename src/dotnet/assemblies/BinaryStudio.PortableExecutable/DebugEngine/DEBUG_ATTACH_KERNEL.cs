using System;

namespace BinaryStudio.PortableExecutable.DebugEngine
    {
    [Flags]
    public enum DEBUG_ATTACH_KERNEL
        {
        DEBUG_ATTACH_KERNEL_CONNECTION = 0x00000000,
        DEBUG_ATTACH_LOCAL_KERNEL      = 0x00000001,
        DEBUG_ATTACH_EXDI_DRIVER       = 0x00000002,
        DEBUG_ATTACH_INSTALL_DRIVER    = 0x00000004
        }
    }