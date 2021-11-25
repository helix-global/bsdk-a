using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct ACL
        {
        public readonly Byte AclRevision;
        public readonly Byte Sbz1;
        public readonly UInt16 AclSize;
        public readonly UInt16 AceCount;
        public readonly UInt16 Sbz2;        
        }
    }