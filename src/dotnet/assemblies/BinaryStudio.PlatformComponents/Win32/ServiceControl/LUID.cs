using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LUID
        {
        public UInt32 LowPart;
        public Int32  HighPart;
        }
    }