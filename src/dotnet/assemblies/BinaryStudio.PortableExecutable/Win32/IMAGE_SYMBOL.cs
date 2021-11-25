using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct IMAGE_SYMBOL
        {
        [FieldOffset( 0)] public unsafe fixed Byte  ShortName[8];
        [FieldOffset( 0)] public unsafe fixed Int32 LongName[2];
        [FieldOffset( 0)] public readonly Int16 Short;
        [FieldOffset( 4)] public readonly Int32 Long;
        [FieldOffset( 8)] public readonly Int32 Value;
        [FieldOffset(12)] public readonly Int16 SectionNumber;
        [FieldOffset(14)] public readonly Int16 Type;
        [FieldOffset(16)] public readonly IMAGE_SYM_CLASS StorageClass;
        [FieldOffset(17)] public readonly Byte NumberOfAuxSymbols;
        }
    }