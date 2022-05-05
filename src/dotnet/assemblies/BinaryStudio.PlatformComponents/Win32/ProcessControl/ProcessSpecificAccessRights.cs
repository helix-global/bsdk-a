using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum ProcessSpecificAccessRights
        {
        Delete                          = (AccessRights.Delete),
        ReadControl                     = (AccessRights.ReadControl),
        WriteDACL                       = (AccessRights.WriteDACL),
        WriteOwner                      = (AccessRights.WriteOwner),
        Synchronize                     = (AccessRights.Synchronize),
        StandardRightsRequired          = (AccessRights.StandardRightsRequired),
        ProcessTerminate                = (0x0001),  
        ProcessCreateThread             = (0x0002),  
        ProcessSetSessionId             = (0x0004),  
        ProcessVirtualMemoryOperation   = (0x0008),  
        ProcessVirtualMemoryRead        = (0x0010),  
        ProcessVirtualMemoryWrite       = (0x0020),  
        ProcessDuplicateHandle          = (0x0040),  
        ProcessCreateProcess            = (0x0080),  
        ProcessSetQuota                 = (0x0100),  
        ProcessSetInformation           = (0x0200),  
        ProcessQueryInformation         = (0x0400),  
        ProcessSuspendResume            = (0x0800),  
        ProcessQueryLimitedInformation  = (0x1000),  
        ProcessSetLimitedInformation    = (0x2000),
        ProcessAllAccess                = StandardRightsRequired|Synchronize|0xffff,
        AccessSystemSecurity            = (AccessRights.AccessSystemSecurity),
        GenericRead                     = (AccessRights.GenericRead),
        GenericWrite                    = (AccessRights.GenericWrite),
        GenericExecute                  = (AccessRights.GenericExecute)
        }
    }