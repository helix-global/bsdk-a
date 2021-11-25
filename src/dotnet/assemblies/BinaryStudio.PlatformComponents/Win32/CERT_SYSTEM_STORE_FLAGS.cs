using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CERT_SYSTEM_STORE_FLAGS
        {
        CERT_SYSTEM_STORE_CURRENT_USER               = unchecked((Int32)0x00010000),
        CERT_SYSTEM_STORE_CURRENT_SERVICE            = unchecked((Int32)0x00040000),
        CERT_SYSTEM_STORE_LOCAL_MACHINE              = unchecked((Int32)0x00020000),
        CERT_SYSTEM_STORE_LOCAL_MACHINE_GROUP_POLICY = unchecked((Int32)0x00080000),
        CERT_SYSTEM_STORE_CURRENT_USER_GROUP_POLICY  = unchecked((Int32)0x00070000),
        CERT_SYSTEM_STORE_SERVICES                   = unchecked((Int32)0x00050000),
        CERT_SYSTEM_STORE_USERS                      = unchecked((Int32)0x00060000),
        CERT_SYSTEM_STORE_LOCAL_MACHINE_ENTERPRISE   = unchecked((Int32)0x00090000),
        CERT_SYSTEM_STORE_RELOCATE_FLAG              = unchecked((Int32)0x80000000),
        CERT_SYSTEM_STORE_LOCATION_MASK              = unchecked((Int32)0x00FF0000),
        CERT_PHYSICAL_STORE_PREDEFINED_ENUM_FLAG     = unchecked((Int32)0x00000001),
        }
    }