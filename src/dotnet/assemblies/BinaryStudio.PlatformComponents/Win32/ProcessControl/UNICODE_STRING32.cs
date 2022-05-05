using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UNICODE_STRING32
        {
        public readonly Int16 Length;
        public readonly Int16 MaximumLength;
        public readonly Int32 Buffer;
        }
    }