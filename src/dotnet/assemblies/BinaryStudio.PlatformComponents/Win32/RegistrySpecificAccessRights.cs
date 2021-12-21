using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [Flags]
    public enum RegistrySpecificAccessRights
        {
        Delete                 = (AccessRights.Delete),
        ReadControl            = (AccessRights.ReadControl),
        WriteDACL              = (AccessRights.WriteDACL),
        WriteOwner             = (AccessRights.WriteOwner),
        Synchronize            = (AccessRights.Synchronize),
        StandardRightsRequired = (AccessRights.StandardRightsRequired),
        AccessSystemSecurity   = (AccessRights.AccessSystemSecurity),
        GenericRead            = (AccessRights.GenericRead),
        GenericWrite           = (AccessRights.GenericWrite),
        GenericExecute         = (AccessRights.GenericExecute),
        QueryValue             = 0x0001,
        SetValue               = 0x0002,
        CreateSubKey           = 0x0004,
        EnumerateSubKeys       = 0x0008,
        Notify                 = 0x0010,
        CreateLink             = 0x0020,
        WoW64_32Key            = 0x0200,
        WoW64_64Key            = 0x0100,
        WoW64_Res              = 0x0300,
        Read                   = ((AccessRights.StandardRightsRead|QueryValue|EnumerateSubKeys|Notify) & (~Synchronize))
        }
    }