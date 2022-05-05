using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LIST_ENTRY32
        {
        public readonly UInt32 FLink;
        public readonly UInt32 BLink;
        }
    }