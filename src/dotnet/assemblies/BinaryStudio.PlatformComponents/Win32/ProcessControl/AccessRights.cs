using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum AccessRights
        {
        Delete                  = (0x00010000),
        ReadControl             = (0x00020000),
        WriteDACL               = (0x00040000),
        WriteOwner              = (0x00080000),
        Synchronize             = (0x00100000),
        StandardRightsRequired  = (0x000f0000),
        StandardRightsRead      = (ReadControl),
        StandardRightsWrite     = (ReadControl),
        StandardRightsExecute   = (ReadControl),
        StandardRightsAll       = (0x001f0000),
        SpecificRightsAll       = (0x0000ffff),
        AccessSystemSecurity    = (0x01000000),
        GenericRead             = unchecked((Int32)(0x80000000)),
        GenericWrite            = (0x40000000),
        GenericExecute          = (0x20000000),
        GenericAll              = (0x10000000)
        }
    }