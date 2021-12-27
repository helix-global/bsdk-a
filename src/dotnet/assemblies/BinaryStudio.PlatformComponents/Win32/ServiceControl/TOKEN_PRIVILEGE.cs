using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TOKEN_PRIVILEGE
        {
        public Int32 PrivilegeCount;
        public LUID_AND_ATTRIBUTES Privilege;
        }
    }