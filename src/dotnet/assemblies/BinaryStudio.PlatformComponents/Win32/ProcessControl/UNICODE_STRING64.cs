using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UNICODE_STRING64
        {
        public readonly UInt16 Length;
        public readonly UInt16 MaximumLength;
        public readonly UInt64 Buffer;
        }
    }