using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct CALLSITEINFO
        {
        [FieldOffset(0)] public readonly UInt32 off;                // offset of call site
        [FieldOffset(4)] public readonly UInt16 sect;               // section index of call site
        [FieldOffset(8)] public readonly DEBUG_TYPE_ENUM TypeIndex;
        }
    }