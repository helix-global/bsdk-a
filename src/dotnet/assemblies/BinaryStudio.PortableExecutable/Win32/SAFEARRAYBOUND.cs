using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    internal struct SAFEARRAYBOUND
        {
        public readonly UInt32 Elements;
        public readonly Int32  LowerBound;
        }
    }