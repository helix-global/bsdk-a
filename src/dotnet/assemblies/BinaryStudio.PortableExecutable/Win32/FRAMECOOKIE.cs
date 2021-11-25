using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct FRAMECOOKIE
        {
        public readonly UInt32 off;                // Frame relative offset
        public readonly UInt16 reg;                // Register index
        public readonly CV_COOKIETYPE cookietype;  // Type of the cookie
        public readonly Byte flags;                // Flags describing this cookie        
        }
    }