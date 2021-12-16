using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UNICODE_STRING32
        {
        public readonly UInt16 Length;
        public readonly UInt16 MaximumLength;
        public readonly UInt32 Buffer;
        }
    }