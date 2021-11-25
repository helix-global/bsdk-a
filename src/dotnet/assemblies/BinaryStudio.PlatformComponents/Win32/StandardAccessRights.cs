using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum StandardAccessRights
        {
        Delete                           = 0x00010000,
        ReadPermissions                  = 0x00020000,
        ChangePermissions                = 0x00040000,
        TakeOwnership                    = 0x00080000,
        Synchronize                      = 0x00100000
        }
    }