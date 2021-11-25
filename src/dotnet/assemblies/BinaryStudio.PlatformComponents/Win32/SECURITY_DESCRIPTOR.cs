using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_DESCRIPTOR
        {
        public readonly Byte Revision;
        public readonly Byte Sbz1;
        public readonly SECURITY_DESCRIPTOR_CONTROL Control;
        public readonly IntPtr Owner;
        public readonly IntPtr Group;
        public readonly unsafe ACL* Sacl;
        public readonly unsafe ACL* Dacl;
        }
    }