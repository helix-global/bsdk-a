using System;

namespace BinaryStudio.PlatformComponents.Win32
    {
    internal struct TOKEN_PRIVILEGE
        {
        public Int32 PrivilegeCount;
        public LUID_AND_ATTRIBUTES Privilege;
        }
    }