using System;

namespace Microsoft.Win32
    {
    [Flags]
    internal enum WindowStyles : uint {
        Popup       = 0x80000000,
        Disabled    = 0x08000000,
        Topmost     = 0x00000008,
        Transparent = 0x00000020
        }
    }