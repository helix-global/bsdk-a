using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LIST_ENTRY64
        {
        public readonly UInt64 FLink;
        public readonly UInt64 BLink;
        }
    }